# PGS Test Assignment

Execution took about 18 hours.

What took time:
* Architecture, making a robust system.
* Calculus, making a pretty arc.
* Refactoring, comments, hiding away public fields.

What went fine:
* Finding a trajectory solution that made sense.

What went wrong:
* I ran out of time.
* Code style and comments are not as good as I wanted.
* There are bugs and imperfections that are not fixed.

Specific problems:
* Grenade and Enemy managers do not use pools. They should.
* There are a few cases of overarching information (e.g., Grenade accessing the global config).
* Could use more comments for clarity.
