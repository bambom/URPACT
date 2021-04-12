using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IActionListener
{
	void Update(float deltaTime);
    void OnActionChanging(Data.Action oldAction, Data.Action newAction);
    void OnInputMove();
    void OnHitData(HitData hitData);
    void OnHurt(int damage);
    void OnBuff(UInt32 target, int id);
    void OnFaceTarget();
    void OnAttribChanged(EPA attrib);
}
