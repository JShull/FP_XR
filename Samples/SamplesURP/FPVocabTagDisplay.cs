namespace FuzzPhyte.XR
{    
    using UnityEngine;
    using TMPro;
    using FuzzPhyte.Utility.Meta;
    using FuzzPhyte.Utility.EDU;

    public class FPVocabTagDisplay : MonoBehaviour
    {
        public SpriteRenderer BackDrop;
        public TextMeshPro MainTextDisplay;
        public TextMeshPro SecondaryTextDisplay;
        public FP_Tag TagData;
        public FP_Vocab VocabData;
        public TextAlignmentOptions TagPivotLocation;
        protected FPLabelTag labelTag;
        [SerializeField]protected Vector3 pivotLocation;
        public Vector3 WorldPivotLocation => pivotLocation;
        public void Start()
        {
            labelTag = new FPLabelTag(TagData, VocabData);
            pivotLocation = ReturnPivotLocation();
        }
        public void DisplayTag()
        {

        }
        public void DisplayVocab()
        {

        }
        public void DisplayTranslation()
        {

        }
        public Vector3 ReturnPivotLocation()
        {
            Vector3[] points = labelTag.GetEdgePoints(BackDrop);
            Vector3 pivot = Vector3.zero;
            switch (TagPivotLocation)
            {
                case TextAlignmentOptions.TopLeft:
                    pivot = points[6];
                    break;
                case TextAlignmentOptions.Top:
                    pivot = points[7];
                    break;
                case TextAlignmentOptions.TopRight:
                    pivot = points[8];
                    break;
                case TextAlignmentOptions.Left:
                    pivot = points[3];
                    break;
                case TextAlignmentOptions.Center:
                    pivot = points[4];
                    break;
                case TextAlignmentOptions.Right:
                    pivot = points[5];
                    break;
                case TextAlignmentOptions.BottomLeft:
                    pivot = points[0];
                    break;
                case TextAlignmentOptions.Bottom:
                    pivot = points[1];
                    break;
                case TextAlignmentOptions.BottomRight:
                    pivot = points[2];
                    break;
            }
            return pivot;

        }
    
    }

}
