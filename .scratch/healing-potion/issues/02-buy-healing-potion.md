# 02: Buy a Healing Potion at the Merchant

**What to build:** The Merchant offers a **Healing Potion** for 30 Gold as menu option `3`; leave moves from `3` to `4`. Buying one deducts 30 Gold, adds one to the potion count and ends the visit (it counts as the visit's one item). If the player can't afford it, the existing can't-afford message appears, Gold is unchanged, and they are asked again. Existing merchant tests that leave with `3` change to `4`. All text in four languages (spec Q11, Q16, Q17, Q18; user stories 7, 8, 9).

**Blocked by:** 01 (Maximum Health Points and the potion count)

**Status:** ready-for-agent

- [ ] The Merchant's offers include the Healing Potion and its price. The leave option reads `4`.
- [ ] Buying a potion with enough Gold deducts 30 Gold, adds one to the count and returns to the path choice.
- [ ] Trying to buy without enough Gold shows can't-afford, leaves Gold unchanged, and asks again.
- [ ] `4` leaves without buying. `3` no longer leaves.
- [ ] Existing merchant tests are updated for the new numbering and pass.
- [ ] New message keys exist in all four languages (the key-parity test passes).
