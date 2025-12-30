using System;

namespace System.Web.UI
{
    public class Page 
    { 
        public Microsoft.AspNetCore.Http.HttpResponse Response { get; set; }
        public Microsoft.AspNetCore.Http.HttpRequest Request { get; set; }
        public ServerUtility Server { get; set; }
        public bool IsPostBack { get; set; }
    }
    
    public class ServerUtility
    {
        public void Transfer(string path) { }
        public string MapPath(string path) { return path; }
    }

    namespace HtmlControls
    {
        public class HtmlForm { }
        public class HtmlGenericControl { }
    }

    namespace WebControls
    {
        public class Label { public string Text { get; set; } }
        public class TextBox { public string Text { get; set; } }
        public class Button { public string Text { get; set; } }
        public class DropDownList { public string SelectedValue { get; set; } public object Items { get; set; } }
        public class GridView { public object DataSource { get; set; } public void DataBind() { } }
        public class Literal { public string Text { get; set; } }
        public class HyperLink { public string Text { get; set; } public string NavigateUrl { get; set; } }
        public class RequiredFieldValidator { public string ErrorMessage { get; set; } }
        public class RegularExpressionValidator { public string ErrorMessage { get; set; } }
        public class CompareValidator { public string ErrorMessage { get; set; } }
        public class ValidationSummary { }
        public class CheckBox { public bool Checked { get; set; } }
        public class RadioButton { public bool Checked { get; set; } }
        public class ListBox { }
        public class Panel { }
        public class Image { public string ImageUrl { get; set; } }
        public class ImageButton { }
        public class LinkButton { public string Text { get; set; } }
        public class Repeater { public object DataSource { get; set; } public void DataBind() { } }
        public class DataList { public object DataSource { get; set; } public void DataBind() { } }
        public class SqlDataSource { }
        public class FileUpload { public bool HasFile { get; set; } public string FileName { get; set; } public void SaveAs(string path) { } }
    }
}

namespace Microsoft.AspNetCore.Http
{
    public class HttpResponse
    {
        public void Write(string text) { }
        public void Redirect(string url) { }
    }
    
    public class HttpRequest
    {
        public string QueryString { get; set; }
    }
}
