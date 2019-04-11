# TODO and other dev comments

The compiler is currently migrating to use Roslyn internally. This is being done so we can rapidly drop in new features for DSharp with minimal ease. 
See the current design and the proposed design below

## Designs

### Current

### Proposed

## TODO
This is a quick list of work left to achieve partity with the existing mono compiler and transfomer. 

- Replace Compiler hot paths with interfaces to break apart systems for IOC
- Decide on IOC Container and perform initial static setup
    - Possibly DryIOC Zero 4.0?
- 