using UnityEngine;

public interface ISenser
{
    bool TryDetect(out GameObject target);
}