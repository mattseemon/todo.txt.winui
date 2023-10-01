# Complete list of credits for Todo
## Source
A big part of the work is inspired by the work of Ben Hughes on [todotxt.net](https://github.com/benrhughes/todotxt.net). While this is mostly a complete re-write, there are blocks of code that were replicated from his work. Hence credit has to be given where credit is due. His work is covered by the FreeBSD License found [here](https://github.com/benrhughes/todotxt.net/blob/dev/BSD_LICENSE.txt).
### Declared License for **todotxt.net**
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

Package|Version|Licenses
---|---:|:---:
[Newtonsoft.Json](#newtonsoftjson)|13.0.3|MIT
[Microsoft.WindowsAppSDK](#microsoftwindowsappsdk)|1.4.1|MIT
[Microsoft.Extensions.Hosting](#microsoftextensionshosting)|7.0.1|MIT
[CommunityToolkit.Mvvm](#communitytoolkitmvvm)|8.2.1|MIT
[Microsoft.Xaml.Behaviors.WinUI.Managed](#microsoftxamlbehaviorswinuimanaged)|2.0.9|MIT
[WinUIEx](#winuiex)|2.3.1|MIT

## Newtonsoft.Json
### Description
Json.NET is a popular high-performance JSON framework for .NET
### Additional Information
|Type|Value|
|---|---|
|Version|13.0.3|
|License|MIT|
|Author|James Newton-King|
|Homepage|https://www.newtonsoft.com/json|
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
[Back to Top](#complete-list-of-credits-for-todo)

---
## Microsoft.WindowsAppSDK 
### Description
The Windows App SDK empowers all Windows Desktop apps with modern Windows UI, APIs, and platform features, including back-compat support.
### Additional Information
|Type|Value|
|---|---|
|Version|1.4.1|
|License|MIT|
|Author|Microsoft|
|Homepage|https://github.com/microsoft/windowsappsdk|
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
[Back to Top](#complete-list-of-credits-for-todo)

---

## Microsoft.Extensions.Hosting
### Description
Hosting and startup infrastructures for applications.
### Additional Information
|Type|Value|
|---|---|
|Version|7.0.1|
|License|MIT|
|Author|Microsoft|
|Homepage|https://github.com/dotnet/runtime|
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
[Back to Top](#complete-list-of-credits-for-todo)

---

## CommunityToolkit.Mvvm
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
### Additional Information
|Type|Value|
|---|---|
|Version|8.2.1|
|License|MIT|
|Author|Microsoft|
|Homepage|https://github.com/CommunityToolkit/dotnet|
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
[Back to Top](#complete-list-of-credits-for-todo)

---

## Microsoft.Xaml.Behaviors.WinUI.Managed
### Description
Easily add interactivity to your WinUI apps using XAML Behaviors. Behaviors encapsulate reusable functionalities for elements that can be easily added to your XAML without the need for more imperative code. This is the managed version for C# UWP WinUI projects.
### Additional Information
|Type|Value|
|---|---|
|Version|2.0.9|
|License|MIT|
|Author|Microsoft|
|Homepage|https://github.com/Microsoft/XamlBehaviors|
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
[Back to Top](#complete-list-of-credits-for-todo)

---

## WinUIEx
### Description
A set of extension methods and classes to fill some gaps in WinUI 3, mostly around windowing.
### Additional Information
|Type|Value|
|---|---|
|Version|2.3.1|
|License|MIT|
|Author|Morten Nielsen|
|Homepage|https://github.com/dotMorten/WinUIEx|
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
[Back to Top](#complete-list-of-credits-for-todo)

---