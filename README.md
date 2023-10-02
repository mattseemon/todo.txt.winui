![Do Not Use](https://img.shields.io/badge/do_not_use-red?style=for-the-badge)
![App Under Construction](https://img.shields.io/badge/app_under_construction-blue?style=for-the-badge)
![Not Production Ready](https://img.shields.io/badge/NOT%20PRODUCTION%20READY-red?style=for-the-badge)


![Todo](./assets/Todo-128.png)

# TODO

An implementation of todo.txt client for Windows.

## Table of Contents
1. [Getting Started](#getting-started)
2. [Installation](#installation)
3. [Development](#development)
4. [Privacy Policy](#privacy-policy)
5. [Contributing](#contributing)
6. [Credits](#credits)
7. [License](#license)

---
This is an implementation of [todo.txt](http://todotxt.org/) for Windows using the .NET Framework and Win UI 3. This is inspired by [todotxt.net](https://github.com/benrhughes/todotxt.net) built by Ben Hughes.

This implementation conforms to the [todo.txt format specfication](https://github.com/todotxt/todo.txt) defined by Gina Trapani.

### Goals
* Full compliance to Gina Trapani [todo.txt format specfication](https://github.com/todotxt/todo.txt)
* Clean & Modern interface
* Completely keyboard driven navigation

## Getting Started

To run this application locally, you can use the following guide.

1. Clone this repository 
   ```bash
   $ git clone https://github.com/mattseemon/todo.txt.winui
   ```
2. Open solution `src/Todo.sln` in Visual Studio 2022 or greater
3. Press `F5` to run application in debug mode or `Ctrl+F5` to run application normally.

[Back to Top](#table-of-contents)
## Installation

Channel | Release
------- | -------
Stable | [![Stable Release](https://img.shields.io/github/v/release/mattseemon/todo.txt.winui?label=%20&logo=windows&style=for-the-badge)](https://github.com/mattseemon/todo.txt.winui/releases/latest)
Pre Release | [![Pre Release](https://img.shields.io/github/v/release/mattseemon/todo.txt.winui?include_prereleases&label=%20&logo=windows&style=for-the-badge)](https://github.com/mattseemon/todo.txt.winui/releases)

 * Future application updates are installed directly from within the App.

[Back to Top](#table-of-contents)
## Development

Todo was developed using the below tools and technologies.
 * [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/)
 * [Windows UI Library in the Windows App SDK (WinUI 3)](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)
 * [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
 * [.NET Framework](https://docs.microsoft.com/en-gb/dotnet/)

### Libraries used

Library | Version 
------- | -------:
[CommunityToolkit.Mvvm ](https://github.com/CommunityToolkit/dotnet)|8.2.1
[CommunityToolkit.WinUI.UI](https://github.com/CommunityToolkit/WindowsCommunityToolkit)|7.1.2
[Microsoft.Extensions.Hosting](https://github.com/dotnet/runtime)|7.0.1
[Microsoft.WindowsAppSDK](https://github.com/microsoft/windowsappsdk)|1.4.1
[Microsoft.Xaml.Behaviors.WinUI.Managed](https://github.com/Microsoft/XamlBehaviors)|2.0.9
[Newtonsoft.Json](https://www.newtonsoft.com/json)|13.0.3
[WinUIEx](https://github.com/dotMorten/WinUIEx)|2.3.1

[Back to Top](#table-of-contents)

## Privacy Policy
I take my own privacy very seriously and I want Todo to be the same. I am not interested in who uses this app and hence will not therefore be collecting or sharing any personal or sensitive data. There are no third-party ads in this app. See privacy policy statement [here](PRIVACY.md).

[Back to Top](#table-of-contents)
## Contributing

If you want to contribute to the project, check out the guidelines article [here](CONTRIBUTING.md). 

[Back to Top](#table-of-contents)
## Credits
 * [Ben Hughes](https://github.com/benrhughes) for the inspiration and some of the code used in this application.
 * Application Icon from the [Kameleon: Rounded icons](https://www.iconfinder.com/search/icons?family=kameleon-icons-rounded) icon set by [Webalys](https://www.iconfinder.com/webalys).
 * Full list of [Credits & 3rd Party Notices](CREDITS.md)

[Back to Top](#table-of-contents)
## License
![License](https://img.shields.io/github/license/mattseemon/Totodo.txt.winuido?style=for-the-badge)

The source code in this repository is covered under the MIT License listed [here](LICENSE]). Feel free to use this in your own projects with attribution!

```
Copyright (c) 2023 Matt Seemon

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```

[Back to Top](#table-of-contents)
## Author

[Matt Seemon](@mattseemon)

[Back to Top](#table-of-contents)

Built with ![Matt Seemon](./assets/heart.png) in Goa, India.