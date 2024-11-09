using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private IMatchGameItem item1;
    private IMatchGameItem item2;

    [SerializeField] MatchGameCore.MatchGameSetup setup;
    private readonly List<IMatchGameItem> items = new List<IMatchGameItem>();

    public GameObject camv2;

    private void Awake()
    {
        MatchGameCore.OnItemSpawned += MatchGameCore_OnItemSpawned;

        MatchGameCore.OnFindMatches += MatchGameCore_OnFindMatches;
        MatchGameCore.OnItemMoveDown += MatchGameCore_OnItemMoveDown;
    }

    private void OnDestroy()
    {
        MatchGameCore.OnItemSpawned -= MatchGameCore_OnItemSpawned;

        MatchGameCore.OnFindMatches -= MatchGameCore_OnFindMatches;
        MatchGameCore.OnItemMoveDown -= MatchGameCore_OnItemMoveDown;
    }

    private void MatchGameCore_OnItemSpawned(Vector2 target, Vector2 start, Vector2Int index, int id)
    {
        items.Add(Item.Instant(target, start, index, id, transform.GetChild(0).GetChild(0)));
    }

    private void MatchGameCore_OnFindMatches(List<Vector2Int> match)
    {
        Sfx.Instant();
        CameraShake.Make();
        BalanceText.Count += match.Count;

        foreach (Vector2Int index in match)
        {
            var find = items.FindItemByIndex(index);
            Vfx.Instant(find.GameObject);

            items.Remove(find);
            Destroy(find.GameObject);
        }
    }

    private void MatchGameCore_OnItemMoveDown(Vector2Int index, Vector2Int nextIndex, Vector2 next)
    {
        var find = items.FindItemByIndex(index);
        if(find == null)
        {
            return;
        }

        find.Index = nextIndex;
        find.TargetPosition = next;
    }

    private void Update()
    {
        if (Result.gameOver || SettingsBtn.IsPressed || ExitPopup.IsOpened || PausePopup.isActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                item1 = hit.collider.GetComponent<IMatchGameItem>();
            }
        }

        if (item1 != null && Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                item2 = hit.collider.GetComponent<IMatchGameItem>();
            }

            if(hit.collider == null)
            {
                item1 = item2 = null;
                return;
            }

            if(item1.ID == item2.ID)
            {
                item1 = item2 = null;
                return;
            }

            SetPositionSfx.Instant();
            item1.ChangeWith(item2);

            item1 = item2 = null;
        }
    }

    public void StartGame()
    {
        camv2.SetActive(true);
        Invoke(nameof(BuildGame), 1.3f);
    }

    public void BuildGame()
    {
        MatchGameCore.MatchGameInit(setup, this);
    }
}
