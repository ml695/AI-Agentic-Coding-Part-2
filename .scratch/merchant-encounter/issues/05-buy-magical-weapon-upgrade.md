# 05: Buy the Magical Weapon upgrade

**What to build:** At the Merchant, choosing `2` buys the **Magical Weapon** upgrade for 120 Gold if the player can afford it. It upgrades the player's current weapon rather than replacing it: +4 Maximum Damage, keeping its occupation-specific type and ASCII art, and shown as magical through a per-language format message (e.g. "magical {0}"), since word order differs by language. Spanish and Italian gender agreement should be avoided by phrasing, or accepted as a known limitation like the missing articles in the Italian ADR. A purchase ends the visit and returns to the path choice (one item per visit). If the player can't afford it, they see a clear message, their Gold is unchanged, and they are asked again. All text in four languages (spec A4, A5, A6; user stories 7, 9, 10, 12, 13, 16).

**Blocked by:** 03 (Merchant on the south path)

**Status:** ready-for-agent

- [ ] Buying the weapon upgrade with enough Gold deducts 120 Gold, raises Maximum Damage by 4, and returns to the path choice.
- [ ] Trying to buy it without enough Gold shows the can't-afford message, leaves Gold unchanged, and asks again.
- [ ] The weapon keeps its type and ASCII art after the upgrade.
- [ ] The character stats and the combat stats show the weapon as magical, with the upgraded Maximum Damage.
- [ ] The player's attack message names the weapon as magical.
- [ ] Without the upgrade, weapon display and damage are unchanged.
- [ ] New message keys exist in all four languages (the key-parity test passes).
