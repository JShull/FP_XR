# FuzzPhyte Unity Tools

## XR

FP_XR is designed and built to be utilized with [FP_Utility](https://github.com/jshull/FP_Utility.git), [FP_Ray](https://github.com/jshull/FP_Ray.git), and [FP_SGraph](https://github.com/jshull/FP_SGraph.git) with the intention of having some core functions that could be helpful in XR projects with 0 dependencies to other XR libraries. Includes a robust set of Controller Event scripts that can be wired up to whatever XR/VR package you're using and with a few lines of code can heavily modify button overlay text/icon information as well as lock the controller. The internal system requires some setup mainly tied to the delegates/events associated with at the controller(s) level or the button(s) level of each controller.

## Setup & Design

**This package supports [FP_Installer](https://github.com/jshull/FP_Installer.git)** - bring the installer package in first to then help automate other git packages.
This package could be used in non-xr projects as a lot of the features/functions are more about spatial layout than anything else.

SamplesURP will require additional package imports.

* com.unity.render-pipelines.universal

### Software Architecture

Stacked components for interactive content like labels moving and labels typing. On the controller interaction side, heavy use of delegates/events that you can subscribe to for when those events fire off. Will require some intervention "wiring" up to the various XR/VR libraries you're using. At this level this just manages the setup and use case across button state changes and controller information. It lets you build out how you want your controller overlay UI information to be presented and all runs via scriptable objects. Just fire up a new one and reset the controller and the entire interface resets. No need to resubscribe to the events - you've already subscribed! - just need to point the system to the new data file and tell it to take action.

### Ways to Extend

## Dependencies

Please see the [package.json](./package.json) file for more information.

## License Notes

See [LICENSE.md](LICENSE.md) for details

## Contact

* [John Shull](mailto:JShull@fuzzphyte.com)

### Additional Notes
