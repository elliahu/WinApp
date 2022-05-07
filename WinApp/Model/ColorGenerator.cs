using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace WinApp.Model
{
    public class ColorGenerator
    {
        public static Color GetColor()
        {
            var posibleColors = new List<Color>()
                {
                    Colors.Red,Colors.Green,Colors.Blue,Colors.Magenta,Colors.Yellow,
                };
            Random random = new Random();

            return posibleColors[random.Next(posibleColors.Count)];
        }
    }
}
