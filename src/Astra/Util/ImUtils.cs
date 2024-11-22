using Astra.DearImGui;

namespace Astra.Util;

internal static unsafe class ImUtils
{
    internal static void SetupImGui()
    {
        ImGui.CreateContext();
        ImGuiIO* io = ImGui.GetIO();
        io->IniFilename = null;
        io->WantSaveIniSettings = false;
        io->ConfigFlags |= ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.ViewportsEnable;
    }
}