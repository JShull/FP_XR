namespace FuzzPhyte.XR.Tests
{
    using System.Reflection;
    using NUnit.Framework;
    using FuzzPhyte.Utility;
    using UnityEngine;

    public class FPXRControllerFeedbackTests
    {
        private GameObject root;
        private FPXRControllerRef button;

        private static FieldInfo Field(System.Type type, string name) =>
            type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Controller feedback regression");
            button = root.AddComponent<FPXRControllerRef>();
            Field(typeof(FPXRControllerRef), "buttonRootParent").SetValue(button, root.transform);
            Field(typeof(FPXRControllerRef), "localButtonRootParent").SetValue(button, root.transform);
            button.SetupUI(Vector3.one, null, Color.white, "", default, true);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(root);

        [TestCase(false)]
        [TestCase(true)]
        public void SilentReleaseRearmsAnalogAudioAndStillUpdatesVisuals(bool hasSource)
        {
            if (hasSource)
            {
                Field(typeof(FPXRControllerRef), "worldAudioRef").SetValue(button, root.AddComponent<AudioSource>());
            }
            var latch = Field(typeof(FPXRControllerRef), "triggerGripAudioAlreadyPlayed");
            latch.SetValue(button, true);

            Assert.That(button.ApplyUIChanges(null, Color.white, "", default,
                true, null, useScale: 2f, vectorData: 0f), Is.True);

            Assert.That(latch.GetValue(button), Is.False);
            Assert.That(root.transform.localScale, Is.EqualTo(Vector3.one * 2f));
        }

        [Test]
        public void HeldAnalogSampleDoesNotRearmAudio()
        {
            var latch = Field(typeof(FPXRControllerRef), "triggerGripAudioAlreadyPlayed");
            latch.SetValue(button, true);
            button.ApplyUIChanges(null, Color.white, "", default, true, null, vectorData: 0.9f);
            Assert.That(latch.GetValue(button), Is.True);
        }

        [Test]
        public void RepeatedLockedFeedbackUsesOneLatchPerButton()
        {
            var feedback = root.AddComponent<FPXRControllerFeedback>();
            var applyLock = typeof(FPXRControllerFeedback).GetMethod("ButtonStateLock",
                BindingFlags.Instance | BindingFlags.NonPublic);
            applyLock.Invoke(feedback, new object[] { button });
            applyLock.Invoke(feedback, new object[] { button });
            var latches = (System.Collections.Generic.HashSet<FPXRControllerRef>)Field(typeof(FPXRControllerFeedback), "lockedFeedbackPlayed")
                .GetValue(feedback);
            Assert.That(latches.Count, Is.EqualTo(1));
        }
    }
}
