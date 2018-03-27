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
    public const int Ancho = 5;
    public const int Alto = 10;
    public const int WorldSize = 100;

    LogicaTablero _logicaTablero;
  

    public const float Distancia = 0.70f;

    //--------ATRIBUTOS--------

    public GameObject tilePrefab;
    public GameObject casaPrefab;
    public GameObject cadaverPrefab;


    public Sprite spriteTierra;
    public Sprite spriteBarro;
    public Sprite spriteAgujero;
    public Sprite spriteSangre;
    public Sprite spriteCasa;


    public Sprite spriteDetective;
    public Sprite spriteArma;
    public Sprite spriteCadaver;

    //public Sprite spriteCasillaIluminada


    //--------ATRIBUTOS--------


    // Use this for initialization
    void Start()
    {
        instance = this;
		//_barcoSeleccionado = null;

		_logicaTablero = new LogicaTablero();
        colocaTablero();


        //_seleccionado = ColorUnidad.ninguno;
        ConstruyeUnidades();
    }

    // Update is called once per frame
    void Update()
    {

    }
		
    //---------------CONSTRUCCIÓN TILES------------------------

    //Pasa la representación lógica del tablero (matriz) a la representación física (gameobjects)
    void colocaTablero()
    {
        GameObject GOTablero = new GameObject("Tablero");

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                //Creamos gameObject
                GameObject GOTileAux = Instantiate(tilePrefab, new Vector3(x * Distancia, -y * Distancia, 0), Quaternion.identity, GOTablero.transform);

				LogicaTile tileAux = _logicaTablero.GetLogicaTile(x, y);

                //SpriteRenderer
                switch (tileAux.GetTerreno())
                {
                    case Terreno.tierra:
                        GOTileAux.GetComponent<SpriteRenderer>().sprite = spriteTierra;
                        break;

                    case Terreno.barro:
                        GOTileAux.GetComponent<SpriteRenderer>().sprite = spriteBarro;
                        break;

                    case Terreno.agujero:
                        GOTileAux.GetComponent<SpriteRenderer>().sprite = spriteAgujero;
                        break;
                }

                //Casilla
				GOTileAux.GetComponent<Tile>().ConstruyeCasilla(tileAux);
            }

        }
        ColocaCasa();
        ColocaCadaver();

    }

    void ColocaAgujero()
    {
        GOTileAux.GetComponent<SpriteRenderer>().sprite = spriteAgujero;
        GOTileAux.GetComponent<Tile>().ConstruyeCasilla(tileAux);
    }
    void ColocaCasa()
    {
        SetPosCasa(new Pos(Random.Range(0, 10), Random.Range(0, 5)));
        GameObject casa = Instantiate(casaPrefab, new Vector3(GetPosCasa().GetX()* Distancia, -GetPosCasa().GetY()*Distancia, 0), Quaternion.identity);
        casa.GetComponent<SpriteRenderer>().sprite = spriteCasa;

    }

    void ColocaCadaver()
    {
        SetPosCadaver(new Pos(Random.Range(0, 10), Random.Range(0, 5)));

        //Compruebo si hay Casa
        while (HayCasa(GetPosCasa(), GetPosCadaver()))
        {
            SetPosCadaver(new Pos(Random.Range(0, 10), Random.Range(0, 5)));
        }
       
        GameObject cadaver = Instantiate(cadaverPrefab, new Vector3(GetPosCadaver().GetX() * Distancia, -GetPosCadaver().GetY() * Distancia, 0), Quaternion.identity);
        cadaver.GetComponent<SpriteRenderer>().sprite = spriteCadaver;

        _logicaTablero.ColocaAgujero();
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
        Pos [] posBarcos = new Pos[3];

        for (int i = 0; i < 3; i++)
			posBarcos[i] = new Pos(-1,-1);
        //Primero colocamos las casillas normales
        //ColocaCasa();
        //ColocaCadaver();
		//CreaDetective("BarcoRojo", ColorUnidad.rojo, spriteBarcoRojo, spriteBarcoRojoSeleccionado,spriteFlechaRoja,ref posBarcos);
		//CreaCadaver("BarcoAzul", ColorUnidad.azul, spriteBarcoAzul, spriteBarcoAzulSeleccionado, spriteFlechaAzul,ref posBarcos);
		//CreaBarco("BarcoVerde", ColorUnidad.verde, spriteBarcoVerde, spriteBarcoVerdeSeleccionado,spriteFlechaVerde,ref posBarcos);
    }
    /*
    void ColocaCasa()
    {

    }
	void CreaDetective(string nombre, Sprite spriteDetective, ref Pos []posBarcos)
    {
		Pos posAux = new Pos(Random.Range(0, 10), Random.Range(0, 10));

        //bool hayBarco = HayBarco(posAux,posBarcos);

		while (_logicaTablero.GetLogicaTile(posAux).GetTerreno() == Terreno.muro || hayBarco)
        {
			posCasa = new Pos(Random.Range(0, 10), Random.Range(0, 10));
            hayBarco = HayBarco(posAux, posBarcos);

        }

		posBarcos [(int)tipoBarco] = posAux;
        GameObject barco = Instantiate(barcoPrefab, new Vector3(posAux.GetX() * Distancia, -posAux.GetY()*Distancia, 0), Quaternion.identity);
        barco.name = nombre;

        LogicaBarco logicaBarco = new LogicaBarco(tipoBarco, posAux);

		//Construcción de flecha
		GameObject flecha = Instantiate(flechaPrefab, new Vector3(posAux.GetX() * Distancia, -posAux.GetY()*Distancia, 0), Quaternion.identity);
		flecha.GetComponent<SpriteRenderer>().sprite = spriteFlecha;

        barco.GetComponent<Barco>().ConstruyeBarco(logicaBarco, spriteBarco, spriteBarcoSeleccionado,flecha);
    }
    //Comprueba si hay barco en una posición
    bool HayBarco(Pos pos, Pos[] posBarcos)
    {
        bool hayBarco = false;

        int i = 0;
        while (!hayBarco && i < 3)
        {
            //Comprobamos si la posicion del barco a colocar coincide con la de un barco ya colocado
            if (posBarcos[i] == pos)
                hayBarco = true;
            i++;
        }
        return hayBarco;
    }

    //---------------CONSTRUCCIÓN UNIDADES------------------------


    public ColorUnidad GetSeleccionado() { return _seleccionado; }

	public void SetSeleccionado(ColorUnidad colBarco, GameObject barco)
    {
        _seleccionado = colBarco;
		_barcoSeleccionado = barco;

    }
		
    public void MoverBarco(Pos pos)
    {
		_barcoSeleccionado.GetComponent<Barco> ().EmpiezaMovimiento(pos);
    }
		
	public LogicaTablero GetLogicaTablero()
	{
		return _logicaTablero;
	}

	public void DeseleccionaBarco()
	{
		_barcoSeleccionado.GetComponent<Barco> ().SetSpriteDeseleccionado ();
		SetSeleccionado(ColorUnidad.ninguno, null);
	}
	public void escribeTiempo(string texto){
		textoReloj.text = "Tiempo: " + texto + "ms";
        
	}
    */
}
