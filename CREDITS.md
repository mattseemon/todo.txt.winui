# Complete list of credits for [Todo](https://github.com/mattseemon/todo.txt.winui)

## Source
A big part of the work is inspired by the work of Ben Hughes on [todotxt.net](https://github.com/benrhughes/todotxt.net). While this is mostly a complete re-write, there are blocks of code that were replicated from his work. Hence credit has to be given where credit is due. His work is covered by the FreeBSD License found [here](https://github.com/benrhughes/todotxt.net/blob/dev/BSD_LICENSE.txt).

### Declared License
  * FreeBSD License
    * Note: This license has also been called the “Simplified BSD License” and the “The 2-Clause BSD License”.
    ```
    All software accompanying this license file is covered via the following FreeBSD-style license.

    Copyright 2011 Ben Hughes. All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are
    permitted provided that the following conditions are met:

      1. Redistributions of source code must retain the above copyright notice, this list of
          conditions and the following disclaimer.

      2. Redistributions in binary form must reproduce the above copyright notice, this list
          of conditions and the following disclaimer in the documentation and/or other materials
          provided with the distribution.

    THIS SOFTWARE IS PROVIDED BY BEN HUGHES ``AS IS'' AND ANY EXPRESS OR IMPLIED
    WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
    FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL BEN HUGHES OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
    ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    The views and conclusions contained in the software and documentation are those of the
    authors and should not be interpreted as representing official policies, either expressed
    or implied, of Ben Hughes
    ```

## Libraries Used
The following 3rd-party software libraries were used to develop **Todo** and are distributed with the same.

Package|Licenses
-------|:--------:
[Newtonsoft.Json (13.0.3)](#newtonsoftjson-1303)|MIT
[Microsoft.WindowsAppSDK (1.3.1)](#microsoftwindowsappsdk-131)|MIT
[Microsoft.Extensions.Hosting (7.0.1)](#microsoftextensionshosting-701)|MIT
[CommunityToolkit.Mvvm (8.2.1)](#communitytoolkitmvvm-821)|MIT
[Microsoft.Xaml.Behaviors.WinUI.Managed (2.0.9)](#microsoftxamlbehaviorswinuimanaged-209)|MIT
[WinUIEx (2.2.0)](#winuiex-220)|MIT

## Newtonsoft.Json (13.0.3)

### Description
Json.NET is a popular high-performance JSON framework for .NET

### Declared License(s)
  * MIT

    ```
    The MIT License (MIT)

    Copyright (c) 2007 James Newton-King

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    ```
### Authors
  * James Newton-King

### Package Homepage
  * https://www.newtonsoft.com/json

[Back to Top](#complete-list-of-credits-for-todo)

---
## Microsoft.WindowsAppSDK (1.3.1)

### Description
The Windows App SDK empowers all Windows Desktop apps with modern Windows UI, APIs, and platform features, including back-compat support.

### Declared License(s)
  * MIT
    ```
    MIT License

    Copyright (c) Microsoft Corporation.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE
    ```
### Authors
  * Microsoft

### Package Homepage
  * https://github.com/microsoft/windowsappsdk

[Back to Top](#complete-list-of-credits-for-todo)

---

## Microsoft.Extensions.Hosting (7.0.1)

### Description
Hosting and startup infrastructures for applications.

### Declared License(s)
  * MIT
    ```
    The MIT License (MIT)

    Copyright (c) .NET Foundation and Contributors

    All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    ```
### Authors
  * Microsoft

### Package Homepage
  * https://github.com/dotnet/runtime

[Back to Top](#complete-list-of-credits-for-todo)

---

## CommunityToolkit.Mvvm (8.2.1)

### Description
This package includes a .NET MVVM library with helpers such as:
      - ObservableObject: a base class for objects implementing the INotifyPropertyChanged interface.
      - ObservableRecipient: a base class for observable objects with support for the IMessenger service.
      - ObservableValidator: a base class for objects implementing the INotifyDataErrorInfo interface.
      - RelayCommand: a simple delegate command implementing the ICommand interface.
      - AsyncRelayCommand: a delegate command supporting asynchronous operations and cancellation.
      - WeakReferenceMessenger: a messaging system to exchange messages through different loosely-coupled objects.
      - StrongReferenceMessenger: a high-performance messaging system that trades weak references for speed.
      - Ioc: a helper class to configure dependency injection service containers.

### Declared License(s)
  * MIT
    ```
    The MIT License (MIT)

    Copyright (c) .NET Foundation and Contributors

    All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy 
    of this software and associated documentation files (the "Software"), to deal 
    in the Software without restriction, including without limitation the rights 
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
    copies of the Software, and to permit persons to whom the Software is 
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all 
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
    SOFTWARE.
    ```
### Authors
  * Microsoft

### Package Homepage
  * https://github.com/CommunityToolkit/dotnet

[Back to Top](#complete-list-of-credits-for-todo)

---

## Microsoft.Xaml.Behaviors.WinUI.Managed (2.0.9)

### Description
Easily add interactivity to your WinUI apps using XAML Behaviors. Behaviors encapsulate reusable functionalities for elements that can be easily added to your XAML without the need for more imperative code. This is the managed version for C# UWP WinUI projects.

### Declared License(s)
  * MIT
    ```
    The MIT License (MIT)

    Copyright (c) 2015 Microsoft

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    ```
### Authors
  * Microsoft

### Package Homepage
  * https://github.com/CommunityToolkit/dotnet

[Back to Top](#complete-list-of-credits-for-todo)

---

## WinUIEx (2.2.0)

### Description
A set of extension methods and classes to fill some gaps in WinUI 3, mostly around windowing.

### Declared License(s)
  * MIT
    ```
    MIT License

    Copyright (c) 2021 Morten Nielsen

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    ```
### Authors
  * Morten Nielsen

### Package Homepage
  * https://github.com/dotMorten/WinUIEx

[Back to Top](#complete-list-of-credits-for-todo)

---