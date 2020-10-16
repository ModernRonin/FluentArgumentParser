# For first release
* make sure extension points are well exposed and documented

* sourcelink
* proper readme/documentation
* CI builds
* protected merges to master, PR, with CI build
* automatic nuget update if release.history changed


# Improvements
improve help-rendering code, instead of local columns have screen-wide columns and enable wrapping that respects columns

# Features
* think about complex properties; eg. a Point; offer mechanism to define how these get transformed to parameters
* think about collection properties (IEnumerable<> etc) - repeated option? or option with separator?
* think about grammar-based API frontend - user supplies just a text-file that looks like the help page
* localization support - ideally, leave this to someone with a PR who's got recent experience with it
* when verb cannot be found, try levensthein distance and propose closest match
* interactive mode where missing arguments are asked from the user, like powershell
* think about allowing inheritance for properties/parameters, for example demo: FeatureCommand.Name helptext


