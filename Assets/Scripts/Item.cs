using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour, IMatchGameItem
{
    public int ID { get; set; }
    public bool IsMove { get; set; }
    public Vector2Int Index { get; set; }
    private Vector2 targetPosition;
    public Vector2 TargetPosition
    {
        get => targetPosition;
        set
        {
            IsMove = true;
            velocity = Vector2.zero;
            targetPosition = value;
        }
    }

    private bool canStart;
    public Vector2 StartPosition { get; set; }

    public GameObject GameObject
    {
        get => gameObject;
    }

    private Vector2 velocity = Vector2.zero;
    private readonly float smoothTime = 0.1f;

    private IEnumerator DelayForMoving()
    {
        yield return new WaitForSeconds((Index.y * 5 + Index.x) * 0.02f);
        canStart = true;
    }

    public static IMatchGameItem Instant(Vector2 target, Vector2 start, Vector2Int _index, int id, Transform grid)
    {
        var _sprite = Resources.Load<Material>($"icons/{id}");
        var _item = Instantiate(Resources.Load<Item>("item"));

        _item.GetComponent<MeshRenderer>().material = _sprite;

        _item.transform.parent = grid;
        _item.transform.localPosition = _item.StartPosition = start;
        _item.transform.localRotation = Quaternion.identity;

        _item.TargetPosition = target;

        _item.ID = id;
        _item.Index = _index;

        _item.StartCoroutine(nameof(DelayForMoving));

        return _item;
    }

    private void Update()
    {
        if (!canStart)
        {
            return;
        }

        transform.localPosition = Vector2.SmoothDamp(transform.localPosition, TargetPosition, ref velocity, smoothTime);
        transform.localRotation = Quaternion.identity;
        IsMove = Vector2.Distance(transform.localPosition, TargetPosition) > 0.001f;
    }
}
