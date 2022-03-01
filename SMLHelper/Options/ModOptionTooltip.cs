namespace SMLHelper.V2.Options
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    internal class ModOptionTooltip : MonoBehaviour, ITooltip
    {
        public string Tooltip;

        void Awake() => Destroy(GetComponent<LayoutElement>());

#if BELOWZERO
        public bool showTooltipOnDrag => true;

        public void GetTooltip(TooltipData tooltip)
        {
            tooltip.prefix.Append(Tooltip);
        }
#else
        public void GetTooltip(out string tooltipText, List<TooltipIcon> _) => tooltipText = Tooltip;
#endif
    }
}
