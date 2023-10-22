using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AbstractWeaponState : MonoBehaviour
{
    public abstract void UseWeapon();

    public virtual void EquipWeapon()
    {
        // pass
    }

    public virtual void UnequipWeapon()
    {
        // pass
    }

    public virtual bool SameWeapon(AbstractWeaponState other)
    {
        return false;
    }

    public virtual bool SameHacksaw(HacksawWeapon other)
    {
        return false;
    }

    public virtual bool SameWhip(WhipWeapon other)
    {
        return false;
    }

    public virtual bool SameJethammer(JethammerWeapon other)
    {
        return false;
    }
}

// represents the hacksaw
public class HacksawWeapon : AbstractWeaponState
{
    public override void UseWeapon()
    {
        // functionality here
    }

    public override bool SameWeapon(AbstractWeaponState other)
    {
        return other.SameHacksaw(this);
    }

    public override bool SameHacksaw(HacksawWeapon other)
    {
        return true;
    }
}

// represents the whip
public class WhipWeapon : AbstractWeaponState
{
    public override void UseWeapon()
    {
        // functionality here
    }

    public override bool SameWeapon(AbstractWeaponState other)
    {
        return other.SameWhip(this);
    }

    public override bool SameWhip(WhipWeapon other)
    {
        return true;
    }
}

// represents the jethammer
public class JethammerWeapon : AbstractWeaponState
{
    public override void UseWeapon()
    {
        // functionality here
    }

    public override bool SameWeapon(AbstractWeaponState other)
    {
        return other.SameJethammer(this);
    }

    public override bool SameJethammer(JethammerWeapon other)
    {
        return false;
    }
}

public class TwoWeaponScript : MonoBehaviour
{
    [SerializeField]
    private List<AbstractWeaponState> availableWeapons;

    // NOTE: do not update these variables; instead, use equippedWeaponIndex
    AbstractWeaponState weapon;
    private int m_index;

    // when updating which weapon is equipped, use this variable
    // NOTE: variable is automatically constrained to be nonnegative and less than the number of available weapons
    private int equippedWeaponIndex
    {
        get
        {
            return this.m_index;
        }
        set
        {
            // NOTE: number of available weapons is never 0
            this.m_index = value % this.availableWeapons.Count;

            this.weapon.UnequipWeapon();
            this.weapon = this.availableWeapons[value];
            this.weapon.EquipWeapon();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.availableWeapons = new List<AbstractWeaponState>();
        this.availableWeapons.Add(gameObject.AddComponent<HacksawWeapon>());

        this.equippedWeaponIndex = 0;
    }

    // switches to the desired weapon depending on player input
    public void HandleWeaponSwap(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            switch (context.ReadValue<int>())
            {
                // if Q is pressed, cycle one weapon to the left
                case -1:
                    this.CycleWeaponLeft();
                    break;
                // if E is pressed, cycle one weapon to the right
                case 1:
                    this.CycleWeaponRight();
                    break;
                // otherwise, invalid input; don't switch weapons
                default:
                    // do nothing
                    break;
            }
        }
    }

    // switches the currently equipped weapon to the weapon to the "left" in the inventory
    private void CycleWeaponLeft()
    {
        this.equippedWeaponIndex -= 1;
    }

    // switches the currently equipped weapon to the weapon to the "right" in the inventory
    private void CycleWeaponRight()
    {
        this.equippedWeaponIndex += 1;
    }

    // use the currently held weapon
    public void UseWeapon()
    {
        this.weapon.UseWeapon();
    }

    private bool IsWeaponAlreadyDiscovered(AbstractWeaponState newWeapon)
    {
        bool discovered = false;
        foreach (AbstractWeaponState weapon in this.availableWeapons)
        {
            if (weapon.SameWeapon(newWeapon))
            {
                discovered = true;
            }
        }
        return discovered;
    }

    // if the given weapon has not already been discovered, add it to the inventory so that
    // it can be used
    public void DiscoverNewWeapon(AbstractWeaponState newWeapon)
    {
        if (!this.IsWeaponAlreadyDiscovered(newWeapon))
        {
            this.availableWeapons.Add(newWeapon);
        }
    }
}
