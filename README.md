> This documentation is in line with the active development, hence should be considered work in progress. To check the documentation for the latest stable version please visit [https://fabiolune.github.io/logic-engine/](https://fabiolune.github.io/logic-engine/)

# Logic Engine

![GitHub CI](https://github.com/fabiolune/logic-engine/actions/workflows/main.yaml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fabiolune_logic-engine&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=fabiolune_logic-engine)
[![codecov](https://codecov.io/gh/fabiolune/logic-engine/branch/main/graph/badge.svg?token=EYWA9ONWVX)](https://codecov.io/gh/fabiolune/logic-engine)
[![NuGet](https://img.shields.io/nuget/v/logic-engine)](https://www.nuget.org/packages/logic-engine/)
[![NuGet](https://img.shields.io/nuget/dt/logic-engine)](https://www.nuget.org/packages/logic-engine/)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Ffabiolune%2Flogic-engine%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/fabiolune/logic-engine/main)

The `logic-engine` is a lightweight .NET library designed to facilitate dynamic and flexible logic systems.
It allows developers to define a generic set of rules that can be compiled into executable code, enabling modifications to business logic without altering the system's core.
By leveraging functional programming principles (thanks to Franco Melandri's [__Tiny FP__](https://github.com/FrancoMelandri/tiny-fp)), it offers tools to evaluate whether an entity satisfies specific conditions and, if not, identify the reasons for failure.
Key components include `Rules`, which are basic logical constructs, `RulesSets` for combined conditions using logical AND, and `RulesCatalog` for broader combinations using logical OR.
It supports a wide range of operators, from comparisons (e.g., equality, greater-than, etc.) to more complex string and enumerable operations.
The library aims to simplify logic handling while maintaining flexibility and clarity in implementation.
