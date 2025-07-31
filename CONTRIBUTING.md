# Contributing to this repository

Few open-source projects are going to be successful without contributions.
This library is no exception and we are deeply grateful for all contributions no matter their size.
However, to improve that collaboration this document presents a few steps to smoothen the process.

## Finding Existing Issues

Before filing a new issue, please search our [issues](https://github.com/dennisdoomen/reflectify/issues) to check if it already exists.

If you do find an existing issue, please include your own feedback in the discussion.
Instead of posting "me too", upvote the issue with 👍, as this better helps us prioritize popular issues and avoids spamming people subscribing to the issue.

### Writing a Good Bug Report

Good bug reports make it easier for maintainers to verify and root cause the underlying problem.
The better a bug report, the faster the problem will be resolved.
Ideally, a bug report should contain the following information:

* A high-level description of the problem.
* A _minimal reproduction_, i.e. the smallest size of code/configuration required to reproduce the wrong behavior.
* A description of the _expected behavior_, contrasted with the _actual behavior_ observed.
* Information on the environment: nuget version, .NET version, etc.
* Additional information, e.g. is it a regression from previous versions? are there any known workarounds?

When ready to submit a bug report, please use the [Bug Report issue template](https://github.com/dennisdoomen/reflectify/issues/new?labels=&template=01_bug_report.yml).

#### Why are Minimal Reproductions Important?

A reproduction lets maintainers verify the presence of a bug, and diagnose the issue using a debugger. A _minimal_ reproduction is the smallest possible console application demonstrating that bug. Minimal reproductions are generally preferable since they:

1. Focus debugging efforts on a simple code snippet,
2. Ensure that the problem is not caused by unrelated dependencies/configuration,
3. Avoid the need to share production codebases.

#### How to Create a Minimal Reproduction

The best way to create a minimal reproduction is gradually removing code and dependencies from a reproducing app, until the problem no longer occurs. A good minimal reproduction:

* Excludes all unnecessary types, methods, code blocks, source files, nuget dependencies and project configurations.
* Contains documentation or code comments illustrating expected vs actual behavior.

## Contributing Changes

In order for this project to provide a consistent experience across the library, we generally want to review every single API that is added, changed or deleted.
Changes to the API must be proposed, discussed and approved with the `api-approved` label in a separate issue before opening a PR.
Sometimes the implementation leads to new knowledge such that the approved API must be reevaluated.
If you're unsure about whether a change fits the library we suggest you open an issue first to avoid wasting your time if the changes does not fit the project.

Also we balance whether proposed features are too niche or complex to pull their weight.
A feature proposal so to speak starts at [-100 points](https://web.archive.org/web/20200112182339/https://blogs.msdn.microsoft.com/ericgu/2004/01/12/minus-100-points/) and needs to prove its worth.
Remember that a rejection of an API approval is not necessarily a rejection of your idea, but merely a rejection of including it in the core library.

When ready to submit a proposal, please use the [API Suggestion issue template](https://github.com/dennisdoomen/reflectify/issues/new?labels=api-suggestion&template=02_api_proposal.yml&title=%5BAPI+Proposal%5D%3A+).

Contributions must also satisfy the other published guidelines defined in this document.

### DOs and DON'Ts

Please do:

* Target the [Pull Request](https://help.github.com/articles/using-pull-requests) at the `main` branch.
* Follow the style presented in the [Coding Guidelines for C#](https://csharpcodingguidelines.com/).
* Ensure that changes are covered by a new or existing set of unit tests which follow the Arrange-Act-Assert syntax.
* Also the code coverage reported by the coveralls must be non-decreasing unless accepted by the authors.
* If the contribution changes the public API, ensure that the changes are properly documented and reviewed.

Please do not:

* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion so we can agree on a direction before you invest a large amount
  of time. This includes _any_ change to the public API.
* Approved API changes are labeled with `api-approved`.