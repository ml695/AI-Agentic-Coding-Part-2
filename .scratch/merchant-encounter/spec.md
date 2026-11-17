# Spec: Merchant encounter on the south path

Status: ready-for-agent

## Problem Statement

The south path is a dead end. Choosing it prints a short "nothing happens, you go home
bored" story and ends the game, so the only meaningful choice in the adventure is whether
to fight the dragon. The player can't prepare for that fight: their character is fixed
once the stats are rolled. A Thief with a d6 dagger, or a character with low Agility,
faces the dragon with no way to improve their odds.

## Solution

The south path leads to a **Merchant** who sells two magical items: **Magical Armor**,
which reduces the damage the dragon deals, and a **Magical Weapon** upgrade, which
increases the player's maximum damage. The player pays with **Gold** rolled during
character creation. They can buy one item if they can afford it, or leave. Afterwards they
return to the path choice, so they can head north and face the dragon better equipped.

## Confirmed decisions

All decisions below are confirmed by the product owner. A1–A6 are the answers to
grilling round 1 (Q1–Q6).

- **Test seams.** The merchant flow is tested end to end through the full game with
  scripted console input. Armor's effect on damage is tested through the dragon's attack.
  Both need repeatable dice, so the game and the player accept a dice roller supplied by
  the caller.
- **A1 — After the merchant (Q1):** the player returns to the path choice. The south
  path no longer ends the game. This reverses the current behavior, where the south path
  ends the game.
- **A2 — Currency (Q2):** Gold is rolled at character creation as a fourth "type 'roll'"
  step: d20 × 10, giving 10–200 Gold. Prices (tunable): Magical Armor **80 Gold**,
  Magical Weapon **120 Gold**.
- **A3 — Protection (Q3):** Magical Armor reduces each dragon hit's damage by a flat
  **3 points**, never below 0. It doesn't change the Agility block roll.
- **A4 — Magical Weapon (Q4):** an *upgrade* to the player's current weapon, not a
  replacement: **+4 Maximum Damage**. The weapon keeps its occupation-specific type and
  ASCII art and is shown as magical.
- **A5 — Purchase limit (Q5):** at most one item per visit. Leaving without buying is
  always allowed.
- **A6 — Languages (Q6):** all new text ships in English, Spanish, French and Italian,
  accent-free, like the existing text.

## User Stories

1. As a player, I want to roll for Gold during character creation, so that I have money to spend on my adventure.
2. As a player, I want my Gold shown in the character stats summary, so that I know what I can afford.
3. As a player, I want to meet a Merchant when I take the south path, so that the south path is worth exploring.
4. As a player, I want the Merchant to tell me what each item does, so that I can make an informed choice.
5. As a player, I want the Merchant to tell me each item's price and how much Gold I have, so that I know which items I can afford.
6. As a player, I want to buy Magical Armor, so that the dragon's hits do less damage to me.
7. As a player, I want to buy a Magical Weapon upgrade, so that my attacks can do more damage to the dragon.
8. As a player, I want to leave the Merchant without buying anything, so that I'm never forced to spend.
9. As a player, I want a clear message when I can't afford an item, so that I understand why the purchase failed and can choose again.
10. As a player, I want my Gold reduced by the price when I buy something, so that the purchase feels real.
11. As a player, I want to return to the path choice after visiting the Merchant, so that I can go north and fight the dragon with my new gear.
12. As a player, I want my stats summary to show my armor and whether my weapon is magical, so that I can see my upgrades.
13. As a player, I want the combat stats shown before the fight to include my armor and upgraded damage, so that I know my odds.
14. As a player, I want a combat message telling me how much damage my armor absorbed, so that I can see it working.
15. As a player, I want a dragon hit smaller than my armor's protection to deal 0 damage instead of healing me, so that the rules make sense.
16. As a player, I want my magical weapon to keep its familiar name and art, shown as magical, so that my character still feels like the occupation I chose.
17. As a player who mistypes at the Merchant, I want to be told the choice is invalid and asked again, so that a typo doesn't waste my visit.
18. As a Spanish, French or Italian player, I want the whole merchant encounter in my language, so that the game stays consistent.
19. As a player who has already bought one item, I want to be stopped from buying a second, so that the one-item rule is enforced.
20. As a player who chooses Exit at the path choice after shopping, I want the game to end normally, so that shopping doesn't break leaving.
21. As a player who goes north without visiting the Merchant, I want the dragon fight to work exactly as before, so that the merchant is optional.
22. As a developer, I want the dice to be replaceable in tests, so that Gold, hits and damage are repeatable and the tests are reliable.
23. As a developer, I want every new message key to exist in all four languages, so that no player sees `[key]` placeholders.

## Implementation Decisions

- **New concepts:** Gold (an amount on the player), Magical Armor (an item that reduces
  damage), Magical Weapon (an upgrade state on the existing weapon), and the Merchant (an
  encounter that offers the items).
- **Player:** gains a Gold amount, rolled after Health Points with the same "type 'roll'"
  prompt pattern, and optional Magical Armor. The stats summary shows Gold, armor ("none"
  or the armor's name), and the weapon's magical status and current Maximum Damage.
- **Weapon:** gains a magical state. Upgrading sets it and raises Maximum Damage by the
  bonus. The display name comes from a per-language format message (e.g. "magical {0}"),
  because word order differs by language.
- **Armor:** a new item with a display name and a damage-reduction amount.
- **Merchant:** a new module that runs one visit. It shows the greeting, the two offers
  with prices, the player's Gold, and a choice menu. It validates the choice, checks
  affordability, applies the purchase to the player and returns.
- **Menu input:** the merchant menu uses numbers (`1` armor, `2` weapon, `3` leave),
  matching the existing number-based menus. Numbers are the same in every language, so
  no translation of commands is needed.
- **Game flow:** the south branch runs the merchant visit and then returns to the path
  choice. The north and exit branches are unchanged.
- **Dragon attack:** after a hit lands and isn't blocked, the damage is reduced by the
  armor's protection, never below 0, before it is subtracted from HP. When armor absorbs
  damage, an extra message reports how much.
- **Dice:** the dice roller becomes replaceable: its roll can be overridden, or it
  implements a small interface. The game accepts an optional dice roller and passes it to
  the player and the dragon. Without one, the game uses the normal random dice, so
  behavior for real players doesn't change.
- **Language data:** add new message keys to all four language sections, and add
  armor/weapon display names to the display maps. Spanish and Italian need gender
  agreement for "magical" (e.g. *espada magica* vs *arco magico*); use phrasing that
  avoids this, or accept the limitation the way the Italian ADR accepts missing articles.
- **Input running out:** if console input ends during the merchant visit, treat it as
  "leave", so the game can't loop forever.

## Testing Decisions

- **What makes a good test:** assert on what the player sees (console output) and on
  observable player state (Gold, HP, Maximum Damage, armor). Don't assert on private
  fields, method call order or message-building internals. Tests use a fixed-roll dice
  roller, so outcomes are deterministic.
- **Seam 1, the full game** (scripted console input through the game's start method):
  - Going south shows the merchant greeting and both offers with prices.
  - Buying armor with enough Gold deducts the price, and the stats show the armor.
  - Buying the weapon with enough Gold deducts the price and raises Maximum Damage by +4.
  - Trying to buy without enough Gold shows the "can't afford" message, leaves Gold
    unchanged, and asks again.
  - Leaving buys nothing, leaves Gold unchanged, and returns to the path choice.
  - Invalid input at the merchant is rejected and asked again.
  - The game ends when input runs out mid-visit, with no infinite loop. Use the existing
    5-second timeout pattern.
  - Going south, then north, reaches the dragon encounter.
- **Seam 2, the dragon's attack:**
  - With armor, an unblocked hit deals its damage minus 3, never below 0, and the
    absorbed-damage message appears.
  - Without armor, the damage is unchanged. This is a regression check.
- **Existing tests to update:** the current south-path test asserts that the game *ends*
  after going south. A1 reverses that, so the test must change to assert the return to
  the path choice.
- **Prior art:** the full-game console tests (scripted input, captured output, timeout
  guard); the dragon tests, which call the dragon's attack directly; and the per-language
  key-parity test, which covers the new message keys in all four languages automatically.

## Out of Scope

- More items, a shop inventory, haggling or selling items back.
- Gold rewards from beating the dragon, or any other way to earn or spend Gold.
- Armor that affects the block (Agility) roll.
- Rules for repeat visits to the Merchant. Every trip south is a normal visit.
- Translating the typed commands (`roll`, `a`/`r`, `n`/`s`/`e`).
- Fixing existing bugs: Halfling Strength or HP below 1, the English-only `<<< claws >>>`
  art, and invalid input during combat counting as an attack.

## Further Notes

- **Balance:** at 80/120 Gold with d20 × 10 Gold, about 65% of characters can afford the
  armor and about 45% the weapon. Tune the prices if that feels wrong.
- **Domain terms** (Gold, Merchant, Magical Armor, Magical Weapon) should go into
  `GLOSSARY.md`.
