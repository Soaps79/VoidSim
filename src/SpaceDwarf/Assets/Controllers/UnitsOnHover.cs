using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Controllers
{
    public interface IUnitEvents : IEventSystemHandler
    {
        void SwapMaterial();
    }

    public class UnitsOnHover : MonoBehaviour, IUnitEvents
    {

        public void SwapMaterial()
        {
            Debug.Log(string.Format("OnHover hit! Object: {0}", name));
        }
    }
}