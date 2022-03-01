#if BELOWZERO
using Text = TMPro.TextMeshPro;
using Font = TMPro.TMP_FontAsset;
using FontStyle = TMPro.FontStyles;
#endif

using UnityEngine;

namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// <para>
    /// Places a simple text object on the screen and keeps it there until either hidden (or a designated fade-to-black timer has passed). 
    /// By default uses the same font/size/color as the "Press Any Button To Begin" message at the beginning of the game, and appears 
    /// centered about 1/3 down the screen, but all parameters can be reconfigured.
    /// </para>
    /// <para>
    /// The idea of the defaults is that new modders don't have to bootstrap a bunch of irritating Unity stuff -- don't have to understand
    /// what a "Material" is or how to make one, don't have to know to initialize a font, or even a color. Can just start adding text and
    /// then can always custom and configure on further revision.
    /// </para>
    /// </summary>
    /// <example>
    /// SIMPLE USAGE EXAMPLE:
    /// BasicText message = new BasicText();
    /// message.ShowMessage("This Message Will Fade In 10 Seconds", 10);
    /// 
    /// COMPLEX USAGE EXAMPLE:
    /// BasicText message = new BasicText(TextAnchor.UpperLeft); // Note many other properties could also be set as constructor parameters
    /// message.setColor(Color.red); // Set Color
    /// message.setSize(20);         // Set Font Size
    /// message.setLoc(200, 400);    // Set x/y position (0,0 is center of screen)
    /// message.setFontStyle(FontStyle.Bold); // Bold 
    /// message.ShowMessage("This message stays on screen until hidden"); // Display message; if fadeout seconds not specified, it just keeps showing
    /// ... // other things happen, time goes by
    /// message.Hide(); // Hides the message
    /// </example>
    public interface IBasicText
    {
        /// <summary>
        /// Resets to using "cloned" font style of Subnautica default
        /// </summary>
        void ClearAlign();

        /// <summary>
        /// Resets to using "cloned" color of Subnautica default.
        /// </summary>
        void ClearColor();

        /// <summary>
        /// Resets to using "cloned" font of Subnautica default.
        /// </summary>
        void ClearFont();

        /// <summary>
        /// Resets to using "cloned" font style of Subnautica default.
        /// </summary>
        void ClearFontStyle();

        /// <summary>
        /// Resets to using "cloned" size of Subnautica default.
        /// </summary>
        void ClearSize();

        /// <summary>
        /// Returns our current text.
        /// </summary>
        /// <returns></returns>
        string GetText();

        /// <summary>
        /// Hides our text item if it is displaying.
        /// </summary>
        void Hide();

        /// <summary>
        /// Sets the text anchor.
        /// </summary>
        /// <param name="useAlign">The text anchor to align to</param>
        void SetAlign(TextAnchor useAlign);

        /// <summary>
        /// Sets the text color
        /// </summary>
        /// <param name="useColor">The text color to use</param>
        void SetColor(Color useColor);

        /// <summary>
        /// Sets the font 
        /// </summary>
        /// <param name="useFont">The font to render the text as.</param>
        void SetFont(Font useFont);

        /// <summary>
        /// Sets the font style.
        /// </summary>
        /// <param name="useStyle">The text font style to use</param>
        void SetFontStyle(FontStyle useStyle);

        /// <summary>
        /// Sets screen display location (position relative to the actual text is determined by the alignment)
        /// </summary>
        /// <param name="set_x">The x coordinate to set</param>
        /// <param name="set_y">The y coordinate to set</param>
        void SetLocation(float set_x, float set_y);

        /// <summary>
        /// Sets the font size.
        /// </summary>
        /// <param name="useSize">The text size to use</param>
        void SetSize(int useSize);

        /// <summary>
        /// Shows our text item, with no schedule fade (i.e. indefinitely)
        /// </summary>
        /// <param name="s">The text to display</param>
        void ShowMessage(string s);

        /// <summary>
        /// Shows our text item, fading after a specified number of seconds (or stays on indefinitely if 0 seconds)
        /// </summary>
        /// <param name="s">The text to display</param>
        /// <param name="seconds">The duration to hold before fading</param>
        void ShowMessage(string s, float seconds);
    }
}
