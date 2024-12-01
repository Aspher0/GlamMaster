using GlamMaster.Structs.Penumbra;
using System;

namespace GlamMaster.Structs.Permissions;

public class PenumbraModPermissions
{
    public readonly string UniqueID;

    public PenumbraMod PenumbraMod;

    public bool CanControlThisMod = false; // This needs to be true for the other permissions to be available.

    public bool CanToggleMod = true;
    public bool CanChangePriority = true;
    public bool CanChangeOptions = true;

    public PenumbraModPermissions(
            PenumbraMod penumbraMod,
            string? uniqueID = null,
            bool canControlThisMod = false,
            bool canToggleMod = true,
            bool canChangePriority = true,
            bool canChangeOptions = true
        )
    {
        if (uniqueID != null)
            UniqueID = uniqueID;
        else
            UniqueID = Guid.NewGuid().ToString();

        PenumbraMod = penumbraMod;

        CanControlThisMod = canControlThisMod;

        CanToggleMod = canToggleMod;
        CanChangePriority = canChangePriority;
        CanChangeOptions = canChangeOptions;
    }
}
