using HarmonyLib;
using Mod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Mod.Extensions
{
    public static class ShopControllerExtensions
    {
        /// <summary>
        /// Get the current stamps in stock.
        /// </summary>
        /// <param name="controller">The controller instance to get the stamps in stock for.</param>
        /// <returns>The current stamps in stock.</returns>
        public static List<ItemInStock> GetStampsInStock(this ShopController controller)
        {
            return Traverse.Create(controller)
                .Field("_stampsInStock")
                .GetValue<ItemInStock[]>()
                .ToList();
        }
    }
}
