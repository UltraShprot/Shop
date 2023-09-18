// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Shop.Data;
using Shop.Interfaces;
using Shop.Models;

namespace Shop.Areas.Identity.Pages.Account.Manage
{
    public class History : PageModel
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ILogger<PersonalDataModel> _logger;
        public IEnumerable<Product> HistoryResult {  get; private set; }
        public History(
            ILogger<PersonalDataModel> logger,
            IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var response = await _purchaseService.GetProductsToPurchaseHistory();
            HistoryResult = response.Data;
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                return Page();
            }
            return Page();
        }
    }
}
