using da.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace da.Objects
{
    public partial class ColorDoorInteract : InteractableArea
    {
        public override void Interact()
        {
            if (Owner is ColorDoor cd)
            {
                if (GameGlobal.Instance.player != null)
                {
                    var save = GameGlobal.Instance.save;
                    var map = cd.FindParent("*Map*");
                    if (map != null && save.MapSaveData.ContainsKey(map.Name))
                    {
                        var mapSave = save.MapSaveData[map.Name];
                        if (mapSave.ContainsKey(cd.Name + "IsUnlocked"))
                        {
                            cd.Open();
                        }
                        else
                        {
                            var player = GameGlobal.Instance.player;
                            if (player.HasItem(cd.UnlockKeyItemId))
                            {
                                if (player.AddItem(cd.UnlockKeyItemId, -1))
                                {
                                    cd.Open();
                                    if (!mapSave.TryAdd(cd.Name + "IsUnlocked", true)) mapSave[cd.Name + "IsUnlocked"] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
