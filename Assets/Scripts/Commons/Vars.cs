﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Vars {

    public static bool EROTIC = false;

    public static int gold;

	public static int freeSortingOrder = 0;

	public static Story.Chapter chapter = Story.Chapter.NONE;

	public static int itemTypeCharsInLine = 20;

	public static Dictionary<HeroType, Hero> heroes = new Dictionary<HeroType, Hero> ();

    public static int describeableId = 0;
}

/* TODO:
	- добавить ещё один слой звёзд starField
	- переделать зум в космосе, чтобы колёсиком мыши не увеличивалось и не уменьшалось звёздное поле, а только корабли и планеты
	- настроить нормальный расчёт цен для предметов
	- добавить объём для типа GOODS
	- планета (или астероид) сначала исследуется (используя навык "Геолог"), когда выясняется какие минералы есть - на неё засылается харвестер, который после сбора возвращается на указанную планету
	- В инвенторе три кнопки - отображать только товары, снаряжение или оборудование
	- В магазине кораблей - либо сделать, чтобы корпуса не повторялись, либо сделать характеристики корпуса рандомными
	- Если сажаться на планету без щитов - корабль получает повреждения
	- Для ремонта корабля дроидом необходима такая штука как scrapMetal - выбивается из врагов и покупается в магазинах
	- На планете есть медцентры в которых можно баффить персонажа
	- На исследование планеты тратяться зонды и исследование планеты занимает время, чем больше зондов, тем быстрее исследуется планета
	или это миниигра, если проиграл - зонд утерян, выиграл - планета исследована, на необитаемых планетах можно оставлять харвестры для сбора ресурсов
	- магазин корпусов - широкий ангар в котором стоят корабли, он прокручивается по горизонтали как бекграунд поанеты и при наведении мыши на корабль появляет информация о его корпусе
*/