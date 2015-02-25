import pygame, sys, random, math, collections, time
from locals import *
from pygame.locals import *

class Snake(pygame.sprite.Sprite):
    def __init__(self, camera):
        pygame.sprite.Sprite.__init__(self)
        self.camera = camera
        self.snakeLeft = pygame.image.load('assets/images/snake_left.png').convert()   #Set Snake sprites
        self.snakeRight = pygame.image.load('assets/images/snake_right.png').convert()
        self.snakeLeft.set_colorkey(self.snakeLeft.get_at((0,0)))             #Choose one pixel and make all pixels that color transparent
        self.snakeRight.set_colorkey(self.snakeRight.get_at((0,0)))
        self.image = self.snakeLeft #Snake starts the game looking left
        self.direction = LEFT
        self.rect = self.image.get_rect()
        self.choice = range(-5,6)        #Made a list of -5 to 5
        self.change = (0,0)
        self.effects = pygame.sprite.Group()
        #Location for not pick-able sprites.
        self.movement_speed = 2
        self.movex = 1
        self.movey = 1
        self.status = collections.defaultdict(lambda: False)

    def kill(self):
        pygame.sprite.Sprite.kill(self)
        pygame.mixer.Sound("assets/audio/spawn.ogg").play()
        for x in range(5):
            snakeActor = BabySnake(self.camera) #Create one snake
            snakeActor.rect.topleft = self.rect.center
            snakes.add(snakeActor) 

    def update(self):
        self.effects.update()
        self.camera.draw(self.effects)
        self.move(self.change[0], self.change[1])
        if random.randint(0,10) == 10:
            self.change = (random.choice(self.choice),random.choice(self.choice))
    
    def move(self, dx, dy):
        mspd = self.movement_speed
        if mspd < 0:
            mspd = 0
        
        self.rect.x += dx*mspd
        self.rect.y += dy*mspd
        if self.rect.bottomright[0] > DOMAIN['x']:
            self.rect.x -= dx*mspd

        if self.rect.bottomright[1] > DOMAIN['y']:
            self.rect.y -= dy*mspd

        if self.rect.topleft[0] < 0:
            self.rect.x -= dx*mspd

        if self.rect.topleft[1] < 0:
            self.rect.y -= dy*mspd

        # Calculate the new position based on randomness plus the control direction
        if dx < 0:
            self.image = self.snakeLeft
            self.direction = LEFT
        elif dx > 0:
            self.image = self.snakeRight
            self.direction = RIGHT

class BabySnake(Snake):
    def __init__(self, camera):
        Snake.__init__(self, camera)
        self.snakeLeft = pygame.image.load('assets/images/baby_snake.png').convert()   #Set Snake sprites
        self.snakeLeft.set_colorkey(self.snakeLeft.get_at((0,0)))             #Choose one pixel and make all pixels that color transparent
        self.snakeRight = pygame.transform.flip(self.snakeLeft, True, False)   #Set Snake sprites
        self.image = self.snakeLeft #Snake starts the game looking left
        self.rect = self.image.get_rect()
        self.choice = range(-5,6)        #Made a list of -5 to 5
        self.change = (0,0)
        self.effects = pygame.sprite.Group()
        #Location for not pick-able sprites.
        self.movement_speed = 3
        self.born = time.time()
        self.seconds_to_adulthood = 50
        self.grown = False

    def hunter_xshift(self):
        hunter = hunters[0]
        if self.rect.x < hunter.rect.x: #Snake is left of hunter
            return 3
        if self.rect.x > hunter.rect.x: #Snake is right of hunter
            return -3
        return 0
        
    def hunter_yshift(self):
        hunter = hunters[0]        
        if self.rect.y < hunter.rect.y: #Snake is above hunter
            return 3
        if self.rect.y > hunter.rect.y: #Snake is below hunter
            return -3
        return 0

    def update(self):
        self.effects.update()
        self.camera.draw(self.effects)
        self.move(self.change[0], self.change[1])
        if random.randint(0,10) == 10:
            self.change = (random.choice(self.choice)+self.hunter_xshift(),random.choice(self.choice)+self.hunter_yshift())
        now = time.time()
        if now > self.born + (self.seconds_to_adulthood/2):
            self.grow()
        if now > self.born + self.seconds_to_adulthood:
            snakeActor = Snake(self.camera) #Create one snake
            snakeActor.rect.topleft = self.rect.center
            snakes.add(snakeActor) 
            self.kill()

    def grow(self):
        if not self.grown:
            self.snakeLeft = pygame.transform.scale(self.snakeLeft, (self.rect.w + 50, self.rect.h + 30))
            self.snakeRight = pygame.transform.flip(self.snakeLeft, True, False)   #Set Snake sprites
            self.grown = True


    def kill(self):
        pygame.mixer.Sound("assets/audio/dead.ogg").play()
        pygame.sprite.Sprite.kill(self)
