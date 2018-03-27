using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    Pos _posCasa, _posCadaver;

     public Pos GetPosCasa() { return _posCasa; }
    public Pos GetPosCadaver() { return _posCadaver; }

   public void SetPosCasa(Pos pos) { _posCasa = pos; }
   public void SetPosCadaver(Pos pos) { _posCadaver = pos; }


    //public Text textoReloj;
    public const int ANCHO = 10;
    public const int ALTO = 5;

    Tablero tablero;
  

    public const float Distancia = 0.70f;

    //--------ATRIBUTOS--------

    public GameObject tilePrefab;
    public GameObject casaPrefab;
    public GameObject cadaverPrefab;
    public GameObject detectivePrefab;
    public GameObject armaPrefab;

    public Sprite spriteBarro;
    public Sprite spriteAgujero;
    public Sprite spriteSangre;
    public Sprite spriteTierra;

    //--------ATRIBUTOS--------


    // Use this for initialization
    void Start()
    {
        instance = this;
		//_barcoSeleccionado = null;

		tablero = new Tablero();
        ColocaTablero();


        //_seleccionado = ColorUnidad.ninguno;
        ConstruyeUnidades();
    }

    // Update is called once per frame
    void Update()
    {

    }
		
    //---------------CONSTRUCCIÓN TILES------------------------

    //Pasa la representación lógica del tablero (matriz) a la representación física (gameobjects)
    void ColocaTablero()
    {
        GameObject GOTablero = new GameObject("Tablero");

        for (int y = 0; y < ALTO; y++)
        {
            for (int x = 0; x < ANCHO; x++)
            {
                //Creamos gameObject
                GameObject GOTileAux = Instantiate(tilePrefab, new Vector3(x * Distancia, -y * Distancia, 0), Quaternion.identity, GOTablero.transform);

				Tile tileAux = tablero.matriz[y, x];


                //Casilla
				GOTileAux.GetComponent<TileView>().ConstruyeCasilla(tileAux);
            }

        }
        ColocaCasa();
        //ColocaCadaver();

    }

    void ColocaAgujero()
    {
       // GOTileAux.GetComponent<SpriteRenderer>().sprite = spriteAgujero;
        //GOTileAux.GetComponent<Tile>().ConstruyeCasilla(tileAux);
    }
    void ColocaCasa()
    {
        SetPosCasa(new Pos(Random.Range(0, 10), Random.Range(0, 5)));
        GameObject casa = Instantiate(casaPrefab, new Vector3(GetPosCasa().x * Distancia, -GetPosCasa().y * Distancia, 0), Quaternion.identity);

    }

    void ColocaCadaver()
    {
        SetPosCadaver(new Pos(Random.Range(0, 10), Random.Range(0, 5)));

        //Compruebo si hay Casa
        while (HayCasa(GetPosCasa(), GetPosCadaver()))
        {
            SetPosCadaver(new Pos(Random.Range(0, 10), Random.Range(0, 5)));
        }
       
        GameObject cadaver = Instantiate(cadaverPrefab, new Vector3(GetPosCadaver().x * Distancia, -GetPosCadaver().y * Distancia, 0), Quaternion.identity);


       //tablero.ColocaAgujero();
    }



    public bool HayCadaver(Pos pos, Pos posCadaver)
    {
        if (pos == posCadaver)
            return true;
        else
            return false;

    }
    public bool HayCasa(Pos pos, Pos posCasa)
    {
        if (pos == posCasa)
            return true;
        else
            return false;
    }
    //---------------CONSTRUCCIÓN TILES------------------------


    //---------------CONSTRUCCIÓN UNIDADES------------------------

    void ConstruyeUnidades()
    {

        //ColocaCasa();
        //ColocaCadaver();
		
    }




		

		



}
