using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public void Move(List<Cell> cells)
    {
        StopAllCoroutines();
        StartCoroutine(WaitToMove(cells));
    }
    private IEnumerator WaitToMove(List<Cell> cells)
    {
        foreach (var cell in cells)
        {
            var pos = new Vector3(cell.RectX-50, cell.RectY-50);
            while (transform.localPosition != pos)
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, pos, 0.1f);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
