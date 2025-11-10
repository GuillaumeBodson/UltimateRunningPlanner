# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade WebUI\WebUI.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
| WorkoutPlanner.API\WorkoutPlanner.API.csproj   | Explicitly excluded by user |
| User.API\User.API.csproj                       | Explicitly excluded by user |
| PaceCalculator.API\PaceCalculator.API.csproj   | Explicitly excluded by user |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                                   |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### WebUI\WebUI.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - (none discovered)

Feature upgrades:
  - (none discovered)

Other changes:
  - (none)
