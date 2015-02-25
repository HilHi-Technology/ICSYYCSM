import pygame, sys, random, math 
from pygame.locals import *
from locals import *
import hunterclass, snakeclass, items, cameraclass

def generateWorld():
    tiles_names = ['assets/images/seamless-background.png',
        'assets/images/seamless-helltile.bmp',
        'assets/images/seamless-jungletile.bmp',
        'assets/images/seamless-skytile.bmp',
        'assets/images/seamless-tile.png']
    volcano = pygame.image.load('assets/images/volocanosplode1.bmp').convert()
    tiles = {image[23:-4]:pygame.image.load(image).convert() for image in tiles_names}
    x,y = DOMAIN.values()
    bkgd = pygame.Surface((x,y))
    rect = pygame.Rect(0,0,1,1)
    while rect.y < y:
        tile = random.choice(tiles.values())
        bkgd.blit(tile,rect.topleft)
        while rect.x < x:
            if random.random() > 0.9:
                tile = random.choice(tiles.values())
            bkgd.blit(tile,rect.topleft)
            rect.x += tile.get_size()[0]
        rect.x = 0
        rect.y += tile.get_size()[1]
    for x in range(200):
        rect.topleft = (random.randint(0,DOMAIN['x']), random.randint(0,DOMAIN['y']))
        bkgd.blit(volcano,rect.topleft)
    return bkgd


def main():
    pygame.init()
    fpsClock = pygame.time.Clock()

    windowSurfaceObj = pygame.display.set_mode((XRES,YRES))      #Set window size
    bkgd = pygame.transform.scale(pygame.image.load('assets/images/bigbg.png').convert(),list(DOMAIN.values()))         #Set background sprite

    hunter = hunterclass.Hunter('assets/images/ash_left.png', 'assets/images/ash_right.png') #Hunter starts the game looking left
    hunters.append(hunter)
    camera = cameraclass.Camera(windowSurfaceObj,hunter)
    hunter.camera = camera
    
    Inventory = pygame.image.load('assets/images/inventory.png')
    
    #Create Leaves
    for x in range(LEAVES_SPAWN):
        leaf = items.Leaf()
        itemSprites.add(leaf)
        leaf.setMovementMod(5)
        if hunter.rect.colliderect(leaf.rect): #If we start with a collision move the leaf
            leaf.rect.x = random.randint(50, DOMAIN['x'])
            leaf.rect.y = random.randint(50, DOMAIN['y'])
            
    #Create FireEggs
    for x in range(FIREEGG_SPAWN):
        egg = items.FireEgg()
        itemSprites.add(egg)
        egg.setMovementMod(5)
        if hunter.rect.colliderect(egg.rect): #If we start with a collision move the egg
            egg.rect.x = random.randint(50, DOMAIN['x'])
            egg.rect.y = random.randint(50, DOMAIN['y'])
        
    #Create firebloom
    for x in range(FIREBLOOM_SPAWN):
        egg = items.FireBloom()
        itemSprites.add(egg)
        egg.setMovementMod(5)
        if hunter.rect.colliderect(egg.rect): #If we start with a collision move the egg
            egg.rect.x = random.randint(50, DOMAIN['x'])
            egg.rect.y = random.randint(50, DOMAIN['y'])
            
    #Create fork
    for i in range(FORK_SPAWN):
        fork = items.Fork()
        itemSprites.add(fork)
        if hunter.rect.colliderect(fork.rect): #Prevent fork from starting on top of player
            fork.rect.x = random.randint(50, DOMAIN['x'])
            fork.rect.y = random.randint(50, DOMAIN['y'])


    for x in range(3):
        snakeActor = snakeclass.Snake(camera) #Create one snake
        snakes.add(snakeActor)

    while True:
        spot = pygame.Rect(camera.screen.topleft,(XRES,YRES))
        windowSurfaceObj.blit(bkgd.subsurface(spot), (0,0)) #Constantly draw background
        windowSurfaceObj.blit(Inventory, (0,630)) #Draw the iventory boxes every refresh
        collide =  pygame.sprite.spritecollide(hunter, itemSprites, True)
        if collide:
            hunter.inventory.add(collide)

        # Only hits center mass effect snake
        def centerMass(projectile,target):
            return projectile.rect.collidepoint(target.rect.center)
        collide =  pygame.sprite.groupcollide(projectiles, snakes, False, False, centerMass)
        for fireball, hit_snakes in collide.items():
            for hit_snake in hit_snakes:
                if fireball.use(hit_snake):
                    projectiles.remove(fireball) 
                    hit_snake.effects.add(fireball)
                    break # Only one can be hit

        collide =  pygame.sprite.spritecollide(hunter, projectiles, False, centerMass)
        for fireball in collide:
            if fireball.use(hunter):
                projectiles.remove(fireball)
                hunter.effects.add(fireball)

        # Snakes can pick up items too
        collide =  pygame.sprite.groupcollide(itemSprites, snakes, True, False, centerMass)
        for item, hit_snakes in collide.items():
            hit_snake = hit_snakes[0] # Only one can have the item
            hit_snake.effects.add(item)
            item.use(hit_snake) # Snakes always use their items right away.

        camera.draw(itemSprites)
        camera.draw(snakes)
        hunter.inventory.draw(windowSurfaceObj)
        camera.draw(projectiles)
        camera.draw(stationary_objects)
        camera.drawsingle(hunter)

        itemSprites.update()
        projectiles.update() # This where the effects do their magic based on the update() function in their item class.
        stationary_objects.update()
        hunter.update()
        snakes.update()
        camera.update()

        if len(snakes) == 0:
            font = pygame.font.Font(None, 48)
            text = font.render("What do you plan to eat now, hmmmn?", 1, (200, 200, 200))
            windowSurfaceObj.blit(text, (200,200))

        for event in pygame.event.get():
            if event.type == QUIT:
                pygame.quit()
                sys.exit()
            if event.type == KEYDOWN:
                if event.key == K_1:
                    hunter.consume(0)
                if event.key == K_2:
                    hunter.consume(1)
                if event.key == K_3:
                    hunter.consume(2)
                if event.key == K_4:
                    hunter.consume(3)
                if event.key == K_5:
                    hunter.consume(4)
                if event.key == K_6:
                    hunter.consume(5)
                if event.key == K_7:
                    hunter.consume(6)
                if event.key == K_8:
                    hunter.consume(7)
                if event.key == K_9:
                    hunter.consume(8)
                if event.key == K_0:
                    hunter.consume(9)
                    
                if event.key == K_SPACE:
                    items.Fork.shoot = 1

                if event.key == K_PERIOD:
                    pygame.mixer.Sound("assets/audio/flawless_victory.wav").play()
                    randnum = random.choice(range(3))
                    if randnum == 0:                        
                        leaf = items.Leaf()
                        hunter.inventory.add(leaf)
                    if randnum == 1:
                        fireegg = items.FireEgg()
                        hunter.inventory.add(fireegg)
                    if randnum == 2:
                        fireBloom = items.FireBloom()
                        hunter.inventory.add(fireBloom)
                
                if event.key == K_ESCAPE:
                    pygame.event.post(pygame.event.Event(QUIT))
                if event.key == K_RIGHT:
                    hunter.moveRight = True
                    hunter.moveLeft = False
                if event.key == K_LEFT:
                    hunter.moveLeft = True
                    hunter.moveRight = False
                if event.key == K_DOWN:
                    hunter.moveDown = True
                    hunter.moveUp = False
                if event.key == K_UP:
                    hunter.moveUp = True
                    hunter.moveDown = False

            if event.type == KEYUP:
                if event.key == K_RIGHT:
                    hunter.moveRight = False
                if event.key == K_LEFT:
                    hunter.moveLeft = False
                if event.key == K_DOWN:
                    hunter.moveDown = False
                if event.key == K_UP:
                    hunter.moveUp = False

        pygame.display.flip()
        fpsClock.tick(FPS)

if __name__ == '__main__':
    main()
