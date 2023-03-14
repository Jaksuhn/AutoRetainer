﻿using AutoRetainer.GcHandin;
using AutoRetainer.Multi;
using Dalamud.Interface.Components;
using ECommons.Configuration;
using ECommons.MathHelpers;
using PInvoke;
using PunishLib.ImGuiMethods;
using System.Windows.Forms;

namespace AutoRetainer.UI;

internal static class Settings
{
    internal static void Draw()
    {
        ImGuiHelpers.ScaledDummy(5f);
        InfoBox.DrawBox("Settings", delegate
        {
            ImGui.SetNextItemWidth(100f);
            ImGui.SliderInt("Time Desynchronization Compensation", ref P.config.UnsyncCompensation.ValidateRange(-60, 0), -10, 0);
            ImGuiComponents.HelpMarker("Additional amount of seconds that will be subtracted from venture ending time to help mitigate possible issues of time desynchronization between the game and your PC. ");
            ImGui.Checkbox("Anonymise Retainers", ref P.config.NoNames);
            ImGuiComponents.HelpMarker("Retainer names will be redacted from general UI elements. They will not be hidden in debug menus and plugin logs however. While this option is on, character and retainer numbers are not guaranteed to be equal in different sections of a plugin (for example, retainer 1 in retainers view is not guaranteed to be the same retainer as in statistics view).");
            ImGui.Checkbox($"Do not use built-in theme", ref P.config.NoTheme);
        });
        InfoBox.DrawBox("Operation", delegate
        {
            if (ImGui.RadioButton("Assign + Reassign", P.config.EnableAssigningQuickExploration && !P.config.DontReassign))
            {
                P.config.EnableAssigningQuickExploration = true;
                P.config.DontReassign = false;
            }
            ImGuiComponents.HelpMarker("Automatically assigns enabled retainers to a Quick Venture if they have none already in progress and reassigns current venture.");
            if (ImGui.RadioButton("Collect", !P.config.EnableAssigningQuickExploration && P.config.DontReassign))
            {
                P.config.EnableAssigningQuickExploration = false;
                P.config.DontReassign = true;
            }
            ImGuiComponents.HelpMarker("Only collect venture rewards from the retainer, and will not reassign them.\nHold CTRL when interacting with the Summoning Bell to apply this mode temporarily.");
            if (ImGui.RadioButton("Reassign", !P.config.EnableAssigningQuickExploration && !P.config.DontReassign))
            {
                P.config.EnableAssigningQuickExploration = false;
                P.config.DontReassign = false;
            }
            ImGuiComponents.HelpMarker("Only reassign ventures that retainers are undertaking.");
            ImGuiComponents.HelpMarker("Hold CTRL to temporarily suppress closing.");
        });
        InfoBox.DrawBox("Quick Retainer Action", delegate
        {
            QRA("Sell Item", ref P.config.SellKey);
            QRA("Entrust Item", ref P.config.EntrustKey);
            QRA("Retrieve Item", ref P.config.RetrieveKey);
            QRA("Put up For Sale", ref P.config.SellMarketKey);
        });
        InfoBox.DrawBox("Statistics", delegate
        {
            ImGui.Checkbox($"Record venture statistics", ref P.config.RecordStats);
        });
        InfoBox.DrawBox("Multi Mode", delegate
        {

            ImGui.Checkbox("Wait for all retainers to be done before logging into character", ref P.config.MultiWaitForAll);
            ImGui.SetNextItemWidth(60);
            ImGui.DragInt("Relog in advance, seconds", ref P.config.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300);
            ImGui.Checkbox("Synchronize retainers (one time)", ref MultiMode.Synchronize);
            ImGuiComponents.HelpMarker("If this setting is on, plugin will wait until all enabled retainers have done their ventures. After that this setting will be disabled automatically and all characters will be processed.");
        });
    }

    static void QRA(string text, ref Keys key)
    {
        ImGui.PushID(text);
        ImGuiEx.TextV($"{text}:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(200f);
        if(ImGui.BeginCombo("##inputKey", $"{key}"))
        {
            var block = false;
            if (ImGui.Selectable("Cancel"))
            {
            }
            if (ImGui.IsItemHovered()) block = true;
            if (ImGui.Selectable("Clear"))
            {
                key = Keys.None;
            }
            if (ImGui.IsItemHovered()) block = true;
            if (!block)
            {
                ImGuiEx.Text(GradientColor.Get(ImGuiColors.ParsedGreen, ImGuiColors.DalamudRed), "Now press new key...");
                foreach (var x in Enum.GetValues<Keys>())
                {
                    if (Bitmask.IsBitSet(User32.GetKeyState((int)x), 15))
                    {
                        ImGui.CloseCurrentPopup();
                        key = x;
                        P.quickSellItems.Toggle();
                        break;
                    }
                }
            }
            ImGui.EndCombo();
        }
        //if (ImGuiEx.EnumCombo($"##{text}", ref key, EnumConsts)) 
        if (key != Keys.None)
        { 
            ImGui.SameLine();
            if (ImGuiEx.IconButton(FontAwesomeIcon.Trash))
            {
                key = Keys.None;
            }
            ImGui.SameLine();
            ImGuiEx.Text("+ right click");
        }
        ImGui.PopID();
    }
}
