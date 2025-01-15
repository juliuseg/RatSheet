using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerAnimation : MonoBehaviour
{
    public Animator markerAnimitor;

    public List<GameObject> markerPieces;

    public Color AttackColor;

    bool AttackMove = false;
    public void PlaceMarker(bool _AttackMove)
    {
        AttackMove = _AttackMove;
        markerAnimitor.Rebind();

        
    }

    private void Update() {
        AnimatorStateInfo stateInfo = markerAnimitor.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("MarkerAnim") && stateInfo.normalizedTime >= 1.0f) {
            // Animation is complete
            markerAnimitor.SetTrigger("NextAnimation");
        } else if (stateInfo.IsName("MarkerAnim") && stateInfo.normalizedTime <= 1.0f) {
            foreach (GameObject piece in markerPieces)
            {
                Color c = piece.GetComponent<SpriteRenderer>().color;
                c.a = stateInfo.normalizedTime;
                piece.GetComponent<SpriteRenderer>().color = c;
            }
        } else {
            foreach (GameObject piece in markerPieces)
            {
                Color c = piece.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                piece.GetComponent<SpriteRenderer>().color = c;
            }
        }

        Color tC = AttackMove ? AttackColor : Color.white; // Target color

        foreach (GameObject piece in markerPieces)
        {
            Color c = piece.GetComponent<SpriteRenderer>().color;
            
            piece.GetComponent<SpriteRenderer>().color = new Color(tC.r, tC.g, tC.b, c.a);
        }
        
    }
}
