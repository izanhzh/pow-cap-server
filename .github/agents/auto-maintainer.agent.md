---
# Fill in the fields below to create a basic custom agent for your repository.
# The Copilot CLI can be used for local testing: https://gh.io/customagents/cli
# To make this agent available, merge this file into the default repository branch.
# For format details, see: https://gh.io/customagents/config

name: Auto Maintainer
description: This agent automatically fixes simple bugs and updates dependencies in the repository.
---

# Auto Maintainer

This agent performs the following tasks:

1. **Bug Fixes**: Automatically applies simple bug fixes based on predefined rules.
2. **Dependency Updates**: Regularly checks for dependency updates and creates pull requests for version upgrades.
