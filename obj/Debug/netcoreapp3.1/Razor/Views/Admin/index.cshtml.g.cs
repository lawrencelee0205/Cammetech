#pragma checksum "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Admin\index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c469e8781408102e71c42faa426d9ccbf49f1c6d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Admin_index), @"mvc.1.0.view", @"/Views/Admin/index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\_ViewImports.cshtml"
using v3x;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\_ViewImports.cshtml"
using v3x.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Admin\index.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c469e8781408102e71c42faa426d9ccbf49f1c6d", @"/Views/Admin/index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"401278cca45ca3702d176ed0ac0a53cebbfdc275", @"/Views/_ViewImports.cshtml")]
    public class Views_Admin_index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<v3x.Models.People>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Admin\index.cshtml"
  
    ViewData["Title"] = "Profile";
    Layout = "_Profile";
    var Name = Context.Session.GetString("Session_Name");

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>");
#nullable restore
#line 9 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Admin\index.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n<p>Username: ");
#nullable restore
#line 10 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Admin\index.cshtml"
        Write(Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<v3x.Models.People> Html { get; private set; }
    }
}
#pragma warning restore 1591
