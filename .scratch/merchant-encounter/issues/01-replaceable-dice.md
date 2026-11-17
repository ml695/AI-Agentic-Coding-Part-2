# 01: Replaceable dice

**What to build:** Make the dice replaceable so tests can supply fixed rolls. The game accepts an optional dice roller and passes it to the player and the dragon; the player uses it for every roll it makes (stats, attack, defense, damage). Without one, the game uses the normal random dice, so real players see no change. This is a prefactor that makes Gold, hits and damage repeatable in later tickets' tests (spec: "Dice", user story 22).

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [x] Tests can create a dice roller that returns a scripted sequence of rolls.
- [x] The game can be started with a supplied dice roller, and that roller drives both the player's and the dragon's rolls.
- [x] The player can be given a dice roller; with none, it uses normal random dice.
- [x] Starting the game with no dice roller behaves exactly as before.
- [x] A test shows that a fixed roller gives repeatable character stats through a full scripted game.
- [x] A test shows that a fixed roller gives a repeatable dragon attack outcome (hit, block or damage).
- [x] All existing tests still pass.
