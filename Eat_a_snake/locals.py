import pygame

FPS = 30
XRES = 1008
YRES = 700
DOMAIN = {'x':3000,'y':3000}

LEAVES_SPAWN = 50
FIREEGG_SPAWN = 50
FIREBLOOM_SPAWN = 50
FORK_SPAWN = 2

LEFT = -1
RIGHT = 1

hunters = []

#Location for not pick-able sprites.
stationary_objects = pygame.sprite.Group()
#Pickupable sprites
itemSprites = pygame.sprite.Group()
#Our lovely villians
snakes = pygame.sprite.Group()
#Shooty things
projectiles = pygame.sprite.Group()
