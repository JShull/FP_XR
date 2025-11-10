# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.6.1] - 2025-11-10

### 0.6.1 Added

### 0.6.1 Changed

- [@JShull](https://github.com/jshull).
  - FP_XRPhysicsEvents.cs
    - way to publicly inject the rigidbody reference
    - way to publicly inject float thresholds

## [0.6.0] - 2025-07-09

### 0.6.0 Added

- [@JShull](https://github.com/jshull).
  - FP_Installer Setup

### 0.6.0 Changed

- [@JShull](https://github.com/jshull).
  - FP_XRControllerRef.cs
    - Checked if our sound source object is enabled before running play

## [0.5.0] - 2025-04-21

### 0.5.0 Added

None

### 0.5.0 Changed

- [@JShull](https://github.com/jshull).
  - FPWorldItem across FPVocabTagDisplay and others now supports an array building sequence for AudioClips

## [0.4.0] - 2025-03-25

### 0.4.0 Added

- [@JShull](https://github.com/jshull).
  - FPXRControllerRef: mono script that points to references at the UI level for images/text etc

### 0.4.0 Changed

- [@JShull](https://github.com/jshull).
  - FPXRControllerEventManager updated all delegates/events tied to an interface setup
  - FPXRControllerFeedback updated all delegates/events tied to an interface setup
  - FPXRControllerRef: all sorts of public methods/functions to modify/access UI items
  - FPXRUtility: removed some references to the ButtonFeedback struct that weren't needed

## [0.3.0] - 2025-03-20

### 0.3.0 Added

- [@JShull](https://github.com/jshull).
  - A new controller manager has been added in to help connect things
  - FPXRControllerEventManager
  - FPXRControllerFeedback
  - FPXRControllerFeedbackConfig

### 0.3.0 Changed

- [@JShull](https://github.com/jshull).
  - FPWorldItem made some items virtual for easier overriding capabilities
  - FPXRTester quick boolean addition to check UnityEvents
  - FPXRUtility added in enums, structs, and other data classes to manage FPXRControllerFeedbackConfig data file

## [0.2.0] - 2024-11-14

See the samples for how a lot of the misc non vr related items can be used without VR libraries.

### 0.2.0 Added

- [@JShull](https://github.com/jshull).
  - A ton of content has been Added
  - FPSocket
  - FPLabelTag
  - FPDistanceCloseItem
  - FPRespawnOnDrop
  - FPSurfaceLock
  - FPWorldCheck
  - FPWorldItem
  - FPXRManager
  - FPXRSocketTag
  - FPXRSpawnPieces
  - FPXRSpawnStack
  - FPXRTag
  - FPXRTester
  - FPXRTool
  - FPXRUtility

### 0.2.0 Changed

- [@JShull](https://github.com/jshull).
  - A majority of everything

## [0.1.0] - 2024-11-04

### 0.1.0 Added

- [@JShull](https://github.com/jshull).
  - Moved all test files to a Unity Package Distribution
  - Setup the ChangeLog.md
  - FP_XR Asmdef
  - SamplesURP

### 0.1.0 Changed

- None... yet

### 0.1.0 Fixed

- Setup the contents to align with Unity naming conventions

### 0.1.0 Removed

- None... yet
