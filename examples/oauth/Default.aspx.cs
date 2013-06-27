﻿// Copyright 2012, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Author: api.anash@gmail.com (Anash P. Oommen)

using Google.Api.Ads.Dfp.Lib;
using Google.Api.Ads.Dfp.v201208;
using Google.Api.Ads.Common.Lib;

using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;

namespace Google.Api.Ads.Dfp.Examples.OAuth {
  /// <summary>
  /// Code-behind class for Default.aspx.
  /// </summary>
  public partial class Default : System.Web.UI.Page {
    /// <summary>
    /// The user for creating services and making DFP API calls.
    /// </summary>
    DfpUser user;

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    void Page_Load(object sender, EventArgs e) {
      user = new DfpUser();
    }

    /// <summary>
    /// Handles the Click event of the btnAuthorize control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    protected void OnAuthorizeButtonClick(object sender, EventArgs e) {
      // This code example shows how to run an DFP API web application
      // while incorporating the OAuth2 web application flow into your
      // application. If your application uses a single Google login to make calls
      // to all your accounts, you shouldn't use this code example. Instead, you
      // should run Common\Util\OAuth2TokenGenerator.cs to generate a refresh
      // token and set that in user.Config.OAuth2RefreshToken field, or set
      // OAuth2RefreshToken key in your App.config / Web.config.
      DfpAppConfig config = user.Config as DfpAppConfig;
      if (config.AuthorizationMethod == DfpAuthorizationMethod.OAuth2) {
        if (user.Config.OAuth2Mode == OAuth2Flow.APPLICATION &&
              string.IsNullOrEmpty(config.OAuth2RefreshToken)) {
          Response.Redirect("OAuthLogin.aspx");
        }
      } else {
        throw new Exception("Authorization mode is not OAuth.");
      }
    }

    /// <summary>
    /// Handles the Click event of the btnGetUsers control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    protected void OnGetUsersButtonClick(object sender, EventArgs e) {
      ConfigureUserForOAuth();

      try {
        // Get the UserService.
        UserService userService = (UserService)user.GetService(DfpService.v201208.UserService);

        // Sets defaults for page and Statement.
        UserPage page = new UserPage();
        Statement statement = new Statement();
        int offset = 0;

        DataTable dataTable = new DataTable();
        dataTable.Columns.AddRange(new DataColumn[] {
            new DataColumn("Serial No.", typeof(int)),
            new DataColumn("User Id", typeof(long)),
            new DataColumn("Email", typeof(string)),
            new DataColumn("Role", typeof(string))
        });
        do {
          // Create a Statement to get all users.
          statement.query = string.Format("LIMIT 500 OFFSET {0}", offset);

          // Get users by Statement.
          page = userService.getUsersByStatement(statement);

          if (page.results != null && page.results.Length > 0) {
            int i = page.startIndex;
            foreach (User usr in page.results) {
              DataRow dataRow = dataTable.NewRow();
              dataRow.ItemArray = new object[] {i + 1, usr.id, usr.email, usr.roleName};
              dataTable.Rows.Add(dataRow);
              i++;
            }
          }
          offset += 500;
        } while (offset < page.totalResultSetSize);
        if (dataTable.Rows.Count > 0) {
          UserGrid.DataSource = dataTable;
          UserGrid.DataBind();
        } else {
          Response.Write("No users were found.");
        }
      } catch (Exception ex) {
        Response.Write(string.Format("Failed to get users. Exception says \"{0}\"",
            ex.Message));
      }
    }

    /// <summary>
    /// Configures the DFP user for OAuth.
    /// </summary>
    private void ConfigureUserForOAuth() {
      DfpAppConfig config = (user.Config as DfpAppConfig);
      if (config.AuthorizationMethod ==  DfpAuthorizationMethod.OAuth2) {
        if (config.OAuth2Mode == OAuth2Flow.APPLICATION &&
              string.IsNullOrEmpty(config.OAuth2RefreshToken)) {
          user.OAuthProvider = (OAuth2ProviderForApplications) Session["OAuthProvider"];
        }
      } else {
        throw new Exception("Authorization mode is not OAuth.");
      }
    }

    /// <summary>
    /// Handles the Click event of the btnLogout control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    protected void OnLogoutButtonClick(object sender, EventArgs e) {
      Session.Clear();
    }

    /// <summary>
    /// Handles the RowDataBound event of the UserGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The
    /// <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance
    /// containing the event data.</param>
    protected void UserGrid_RowDataBound(object sender, GridViewRowEventArgs e) {
      if (e.Row.DataItemIndex >= 0) {
        e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
        e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
        e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;
        e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
      }
    }
  }
}
