CREATE TABLE IF NOT EXISTS `private_rooms` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(150) NOT NULL,
  `rating` int(5) NOT NULL,
  `description` varchar(150) NOT NULL,
  `ownerid` int(150) NOT NULL,
  `status` int(11) NOT NULL,
  `tags` longtext NOT NULL,
  `thumbnail` varchar(150) NOT NULL,
  `petsAllowed` int(11) NOT NULL,
  `category` int(11) NOT NULL,
  `model` varchar(150) NOT NULL,
  `wallpaper` int(11) NOT NULL,
  `floorpaper` int(11) NOT NULL,
  `landscape` varchar(111) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=3 ;