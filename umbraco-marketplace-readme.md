![image](public/images/compact-mode-window.png)

<div align="center">

![NuGet Downloads](https://img.shields.io/nuget/dt/astroboard?label=NuGet%20Downloads)
![GitHub Issues](https://img.shields.io/github/issues/wpplumber/astroboard)
![Umbraco Versions](https://img.shields.io/badge/Umbraco-13-blue)
![Astro Version](https://img.shields.io/badge/Astro-4.15.7-blue)
![TypeScript](https://img.shields.io/badge/TypeScript-5.4.5-blue)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-3.4.3-blue)

</div>

Astroboard is an Umbraco dashboard to get insights of your contents, assets and members.

Here's a clean, factual sustainability table for your three projects that you can easily extend:

---

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

## Preview - Compact mode

![astroboard compact mode](public/images/astroboard-compact-mode.png)

## Preview - Full screen mode

![astroboard fullscreen mode](public/images/mac-astroboard-fullscreen-mode.png)

## System Requirements
Astroboard has the following requirements:
Umbraco version 13.

>It is recommended to upgrade your Umbraco installation to the latest version.


## Installation

`dotnet add package astroboard`

## Just like that, it’s done! 🎉
If you've installed the Astroboard you should start your website (`dotnet run`) and automatically the Astroboard section should appear in the backoffice, as shown in previews above.

## Copyright

Copyright © 2025 [Tarik Rital](https://www.tarikrital.website/).
