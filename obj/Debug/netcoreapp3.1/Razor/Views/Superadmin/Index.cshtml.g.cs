#pragma checksum "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Superadmin\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f1ee2651423383cad9d52d2e1a2273ac7886badf"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Superadmin_Index), @"mvc.1.0.view", @"/Views/Superadmin/Index.cshtml")]
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
#line 1 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Superadmin\Index.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f1ee2651423383cad9d52d2e1a2273ac7886badf", @"/Views/Superadmin/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"401278cca45ca3702d176ed0ac0a53cebbfdc275", @"/Views/_ViewImports.cshtml")]
    public class Views_Superadmin_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<v3x.Models.People>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Superadmin\Index.cshtml"
  
    ViewData["Title"] = "Profile";
    Layout = "_Profile";
    var Name = Context.Session.GetString("Session_Name");

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>Profile</h1>\r\n<p>Username: ");
#nullable restore
#line 10 "C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Views\Superadmin\Index.cshtml"
        Write(Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n\r\n\r\n\r\n");
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
