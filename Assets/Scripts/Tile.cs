using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour {

	LogicaTile _logicaTile;

	// Use this for initialization
	void Start () {
	}
	
	public void ConstruyeCasilla(LogicaTile logicaTile)
    {
		_logicaTile = logicaTile;
    }

    private void OnMouseDown()
    {
		//if (GameManager.instance.GetSeleccionado () == ColorUnidad.ninguno) {
			SpriteRenderer render = GetComponent<SpriteRenderer> ();

			switch (_logicaTile.GetTerreno ()) {
			case Terreno.tierra:
				_logicaTile.SetTerreno (Terreno.tierra);
				render.sprite = GameManager.instance.spriteTierra;

				break;

			case Terreno.barro:
				_logicaTile.SetTerreno (Terreno.barro);
				render.sprite = GameManager.instance.spriteBarro;
				break;

			case Terreno.agujero:
				_logicaTile.SetTerreno (Terreno.agujero);
				render.sprite = GameManager.instance.spriteAgujero;
				break;

			}
		} 
    /*
		else if (_logicaTile.GetTerreno () == Terreno.muro)
		{
			GameManager.instance.DeseleccionaBarco ();
		} 

		//Mover
		else 
		{
            GameManager.instance.MoverBarco(_logicaTile.GetPos());
		}
			
    }
    */
}
