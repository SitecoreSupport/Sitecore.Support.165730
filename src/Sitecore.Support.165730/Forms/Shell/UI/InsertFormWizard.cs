using Sitecore.Data.Items;
using Sitecore.Form.Core.Configuration;
using Sitecore.WFFM.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Sitecore.Support.Forms.Shell.UI
{
  public class InsertFormWizard : Sitecore.Forms.Shell.UI.InsertFormWizard
  {
    protected override bool ActivePageChanging(string page, ref string newpage)
    {
      bool flag = true;
      if (!Settings.Analytics.IsAnalyticsAvailable && (newpage == "AnalyticsPage"))
      {
        newpage = "ConfirmationPage";
      }
      if (base.CheckGoalSettings(page, ref newpage))
      {
        if ((this.InsertForm.Checked && (page == "CreateForm")) && (newpage == "FormName"))
        {
          newpage = "SelectForm";
        }
        if ((this.InsertForm.Checked && (page == "ConfirmationPage")) && (newpage == "AnalyticsPage"))
        {
          newpage = "SelectPlaceholder";
        }
        if ((this.InsertForm.Checked && (page == "SelectForm")) && (newpage == "FormName"))
        {
          newpage = "CreateForm";
        }
        if (((page == "CreateForm") || (page == "FormName")) && (newpage == "SelectForm"))
        {
          if (base.EbFormName.Value == string.Empty)
          {
            Context.ClientPage.ClientResponse.Alert(DependenciesManager.ResourceManager.Localize("EMPTY_FORM_NAME"));
            newpage = (page == "CreateForm") ? "CreateForm" : "FormName";
            return flag;
          }
          if (this.FormsRoot.Database.GetItem(this.FormsRoot.Paths.ContentPath + "/" + base.EbFormName.Value) != null)
          {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("'{0}' ", base.EbFormName.Value);
            builder.Append(DependenciesManager.ResourceManager.Localize("IS_NOT_UNIQUE_NAME"));
            Context.ClientPage.ClientResponse.Alert(builder.ToString());
            newpage = (page == "CreateForm") ? "CreateForm" : "FormName";
            return flag;
          }
          if (!Regex.IsMatch(base.EbFormName.Value, Sitecore.Configuration.Settings.ItemNameValidation, RegexOptions.ECMAScript))
          {
            StringBuilder builder2 = new StringBuilder();
            builder2.AppendFormat("'{0}' ", base.EbFormName.Value);
            builder2.Append(DependenciesManager.ResourceManager.Localize("IS_NOT_VALID_NAME"));
            Context.ClientPage.ClientResponse.Alert(builder2.ToString());
            newpage = (page == "CreateForm") ? "CreateForm" : "FormName";
            return flag;
          }
          if (base.CreateBlankForm.Checked)
          {
            newpage = !string.IsNullOrEmpty(this.Placeholder) ? "ConfirmationPage" : "SelectPlaceholder";
            if (Settings.Analytics.IsAnalyticsAvailable && (newpage == "ConfirmationPage"))
            {
              newpage = "AnalyticsPage";
            }
          }
        }
        if ((page == "SelectForm") && (((newpage == "SelectPlaceholder") || (newpage == "ConfirmationPage")) || (newpage == "AnalyticsPage")))
        {
          string selected = base.multiTree.Selected;
          Item item = StaticSettings.GlobalFormsRoot.Database.GetItem(selected);
          if ((selected == null) || (item == null))
          {
            Context.ClientPage.ClientResponse.Alert(DependenciesManager.ResourceManager.Localize("PLEASE_SELECT_FORM"));
            newpage = "SelectForm";
            return flag;
          }
          if (item.TemplateID != IDs.FormTemplateID)
          {
            StringBuilder builder3 = new StringBuilder();
            builder3.AppendFormat("'{0}' ", item.Name);
            builder3.Append(DependenciesManager.ResourceManager.Localize("IS_NOT_FORM"));
            Context.ClientPage.ClientResponse.Alert(builder3.ToString());
            newpage = "SelectForm";
            return flag;
          }
        }
        if ((newpage == "SelectPlaceholder") && (page == "AnalyticsPage"))
        {
          newpage = string.IsNullOrEmpty(this.Placeholder) ? "SelectPlaceholder" : "SelectForm";
        }
        if (((newpage == "SelectPlaceholder") && (page == "SelectForm")) && !this.InsertForm.Checked)
        {
          newpage = string.IsNullOrEmpty(this.Placeholder) ? "SelectPlaceholder" : (!Settings.Analytics.IsAnalyticsAvailable ? "ConfirmationPage" : "AnalyticsPage");
        }
        if (((newpage == "SelectPlaceholder") && (page == "SelectForm")) && this.InsertForm.Checked)
        {
          newpage = string.IsNullOrEmpty(this.Placeholder) ? "SelectPlaceholder" : "ConfirmationPage";
        }
        if (((page == "ConfirmationPage") && (newpage == "ConfirmationPage")) && !Settings.Analytics.IsAnalyticsAvailable)
        {
          newpage = string.IsNullOrEmpty(this.Placeholder) ? "SelectPlaceholder" : "SelectForm";
        }
        if ((page == "ConfirmationPage") && ((newpage == "SelectPlaceholder") || (newpage == "AnalyticsPage")))
        {
          if (newpage != "AnalyticsPage")
          {
            newpage = string.IsNullOrEmpty(this.Placeholder) ? "SelectPlaceholder" : "SelectForm";
          }
          base.NextButton.Disabled = false;
          base.BackButton.Disabled = false;
          base.CancelButton.Header = "Cancel";
          base.NextButton.Header = "Next >";
        }
        if ((page == "SelectPlaceholder") && ((newpage == "ConfirmationPage") || (newpage == "AnalyticsPage")))
        {
          if (string.IsNullOrEmpty(this.ListValue))
          {
            newpage = "SelectPlaceholder";
            Context.ClientPage.ClientResponse.Alert(DependenciesManager.ResourceManager.Localize("SELECT_MUST_SELECT_PLACEHOLDER"));
          }
          else if (this.InsertForm.Checked)
          {
            newpage = "ConfirmationPage";
          }
        }
        if (((((page == "ConfirmationPage") || (page == "AnalyticsPage")) && (newpage == "SelectForm")) || ((page == "SelectPlaceholder") && (newpage == "SelectForm"))) && base.CreateBlankForm.Checked)
        {
          newpage = "CreateForm";
        }
        if (newpage == "ConfirmationPage")
        {
          base.ChoicesLiteral.Text = this.RenderSetting();
        }
      }
      return flag;
    }

  }
}