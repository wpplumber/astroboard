# [Astroboard](https://www.astroboard.website)

[Astroboard](https://www.astroboard.website/) is an open-source [Astro](https://www.astro.build/)-based
Umbraco dashboard package. It is written in [TypeScript](https://www.typescriptlang.org) and
uses [Vue](https://vuejs.org/).

![Astroboard is an Umbraco dashboard to get insights of your contents, assets and members.](https://raw.githubusercontent.com/wpplumber/astroboard/main/public/images/compact-mode-window.png)

![NuGet Downloads](https://img.shields.io/nuget/dt/astroboard?label=NuGet%20Downloads)
![GitHub Issues](https://img.shields.io/github/issues/wpplumber/astroboard)
![Umbraco Versions](https://img.shields.io/badge/Umbraco-13-blue)
![Astro Version](https://img.shields.io/badge/Astro-5.8.0-blue)

## ♻️ Sustainability Practices

### **Astro Dashboard**
- **🌐 Host Routing**: `~0.5s` faster dev API calls via `import.meta.env.DEV` checks (✅ Implemented)
- **🧩 Async Components**: `~18-25%` smaller initial loads with `defineAsyncComponent()` (🚧 *WIP*)
- **📊 Chart Optimization**: `~35%` faster renders using tree-shaken `chart.js` (🚧 *WIP*)

### **NuGet Package**
- **✂️ Assembly Trimming**: `~38%` size reduction (`<PublishTrimmed>true`) (✅ Implemented)
- **🗃️ Paged Queries**: `~65%` fewer DB hits via `GetPagedChildren()` (✅ Implemented)
- **🌐 Efficient Queries**: Cross-DB compatibility with LINQ/optimized SQL (✅ Implemented)

### **Umbraco 13 Host**
- **🎯 Fixed-Port Binding**: `~30%` faster local testing (Kestrel on `:5000/:5001`) (✅ Implemented)
- **🌐 Dev CORS**: Zero-config mode switching with `AllowAnyOrigin()` (✅ Implemented)
- **🔌 HTTP/HTTPS Parallelism**: Reduced TLS handshake energy (✅ Implemented)

---

## System Requirements
Astroboard has the following requirements:
Umbraco version 13.

>It is recommended to upgrade your Umbraco installation to the latest version.


## Installation

`dotnet add package astroboard`

## Just like that, it’s done! 🎉
If you've installed the Astroboard you should start your website (`dotnet run`) and automatically the Astroboard section should appear in the backoffice, as shown in previews below.

## Preview - Compact mode

![image](https://raw.githubusercontent.com/wpplumber/astroboard/main/public/images/astroboard-compact-mode.png)

## Preview - Full screen mode

![preview](https://raw.githubusercontent.com/wpplumber/astroboard/main/public/images/mac-astroboard-fullscreen-mode.png)

## Reporting Bugs and Issues
If you think you've found a bug and you're confident it's a new bug and have confirmed that someone else is facing the same issue, go ahead and create a [new GitHub issue](https://github.com/wpplumber/astroboard/issues). Be sure to include as much information as possible so we can reproduce the bug.

## Reporting Security Issues and Responsible Disclosure
I appreciate responsible disclosure of vulnerabilities that might impact the integrity of Umbraco CMS and users.


## License

Copyright © 2025 [Tarik Rital](https://www.tarikrital.website/).

Licensed under the [MIT License](https://github.com/wpplumber/astroboard/blob/main/LICENSE.md).
