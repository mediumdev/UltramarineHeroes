using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Скрипт отвечает за проигрывание VisualEffects в инвентах анимации
 * В скприпте находиться два метода PlayVisualEffects и PlayVisualEffectsUp - которые отвечают за запуск FX. 
 * Как это работает - Данный скрипт нужно прикрепить к объекту на котором есть компоненты Аниматор или Анмация, после чего зайти в 
 * нужную анимацию и в ней добавить эвент и в поле функции выбрать данный метод (PlayVisualEffects) либо вбиваем его ручками
 * Дополнительно сам VisualEffects должен находиться в иерархии объекта
 */
public class AnimatorEvents : MonoBehaviour
{
   [SerializeField] private ParticleSystem _fx_VisualEffects;
   [SerializeField] private ParticleSystem _fx_VisualEffectsUp;
   [SerializeField] private GameObject _gameOjeckt;

    public void PlayVisualEffects() // данный метод нужны выбрать в фунциях саммого инвента
        {
           _fx_VisualEffects.Play();
        }
    public void PlayVisualEffectsUp() // данный метод нужны выбрать в фунциях саммого инвента
        {
           _fx_VisualEffectsUp.Play();
        }
    public void DisableGameOject()
    {
        _gameOjeckt.SetActive(false);
    }

}
