// Compatibility stubs for System.Web.UI types
// This file provides minimal type definitions to enable compilation
// Web Forms functionality should be migrated to ASP.NET Core

namespace System.Web.UI
{
    public class Page
    {
        public HttpRequest Request { get; } = new HttpRequest();
        public HttpResponse Response { get; } = new HttpResponse();
        public HttpServerUtility Server { get; } = new HttpServerUtility();
        public HttpSessionState Session { get; } = new HttpSessionState();
        public HttpApplicationState Application { get; } = new HttpApplicationState();
        public bool IsPostBack { get; set; }
    }

    public class UserControl { }

    public class Control { }
}

namespace System.Web.UI.WebControls
{
    public class TextBox : System.Web.UI.Control
    {
        public string Text { get; set; } = "";
    }

    public class Label : System.Web.UI.Control
    {
        public string Text { get; set; } = "";
    }

    public class Button : System.Web.UI.Control { }

    public class LinkButton : System.Web.UI.Control { }

    public class HyperLink : System.Web.UI.Control
    {
        public string NavigateUrl { get; set; } = "";
        public string Text { get; set; } = "";
    }

    public class DropDownList : System.Web.UI.Control
    {
        public string SelectedValue { get; set; } = "";
        public string Text { get; set; } = "";
        public ListItemCollection Items { get; } = new ListItemCollection();
    }

    public class GridView : System.Web.UI.Control
    {
        public object? DataSource { get; set; }
        public void DataBind() { }
    }

    public class Repeater : System.Web.UI.Control
    {
        public object? DataSource { get; set; }
        public void DataBind() { }
    }

    public class Image : System.Web.UI.Control
    {
        public string ImageUrl { get; set; } = "";
    }

    public class FileUpload : System.Web.UI.Control
    {
        public bool HasFile { get; set; }
        public string FileName { get; set; } = "";
        public void SaveAs(string path) { }
    }

    public class SqlDataSource : System.Web.UI.Control
    {
        public object? DataSource { get; set; }
    }

    public class RegularExpressionValidator : System.Web.UI.Control
    {
        public string ControlToValidate { get; set; } = "";
        public string ValidationExpression { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
    }

    public class ListItemCollection
    {
        public void Add(ListItem item) { }
        public void Clear() { }
    }

    public class ListItem
    {
        public string Text { get; set; } = "";
        public string Value { get; set; } = "";

        public ListItem() { }
        public ListItem(string text, string value)
        {
            Text = text;
            Value = value;
        }
    }
}

namespace System.Web.UI.HtmlControls
{
    public class HtmlForm : System.Web.UI.Control
    {
        public string Action { get; set; } = "";
    }

    public class HtmlGenericControl : System.Web.UI.Control { }

    public class HtmlInputButton : System.Web.UI.Control { }

    public class HtmlInputText : System.Web.UI.Control
    {
        public string Value { get; set; } = "";
    }

    public class HtmlImage : System.Web.UI.Control
    {
        public string Src { get; set; } = "";
    }
}

namespace System.Web
{
    public class HttpRequest
    {
        public System.Collections.Specialized.NameValueCollection QueryString { get; } = new System.Collections.Specialized.NameValueCollection();
        public System.Collections.Specialized.NameValueCollection Form { get; } = new System.Collections.Specialized.NameValueCollection();
    }

    public class HttpResponse
    {
        public void Redirect(string url) { }
        public void Write(string text) { }
    }

    public class HttpServerUtility
    {
        public string MapPath(string path) => path;
        public void Transfer(string path) { }
    }

    public class HttpSessionState
    {
        private System.Collections.Generic.Dictionary<string, object?> _data = new System.Collections.Generic.Dictionary<string, object?>();

        public object? this[string key]
        {
            get => _data.ContainsKey(key) ? _data[key] : null;
            set => _data[key] = value;
        }
    }

    public class HttpApplicationState
    {
        private System.Collections.Generic.Dictionary<string, object?> _data = new System.Collections.Generic.Dictionary<string, object?>();

        public object? this[string key]
        {
            get => _data.ContainsKey(key) ? _data[key] : null;
            set => _data[key] = value;
        }
    }

    public class HttpContext
    {
        public static HttpContext? Current { get; set; }
        public HttpRequest Request { get; } = new HttpRequest();
        public HttpResponse Response { get; } = new HttpResponse();
        public HttpSessionState Session { get; } = new HttpSessionState();
    }
}
