import React, { Fragment } from 'react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import african from '../../img/cuisines/african.jpg';
import american from '../../img/cuisines/american.jpg';
import british from '../../img/cuisines/british.webp';
import caribbean from '../../img/cuisines/caribbean.webp';
import chinese from '../../img/cuisines/chinese.jpg';
import easteuropean from '../../img/cuisines/easteuropean.jpg';
import french from '../../img/cuisines/french.jpg';
import greek from '../../img/cuisines/greek.jpg';
import indian from '../../img/cuisines/indian.jpg';
import italian from '../../img/cuisines/italian.jpg';
import japanese from '../../img/cuisines/japanese.jpg';
import korean from '../../img/cuisines/korean.jpg';
import mexican from '../../img/cuisines/mexican.jpg';
import middleeastern from '../../img/cuisines/middleeastern.jpg';
import pakistani from '../../img/cuisines/pakistani.webp';
import portuguese from '../../img/cuisines/portuguese.jpg';
import southamerican from '../../img/cuisines/southamerican.jpg';
import spanish from '../../img/cuisines/spanish.jpg';
import thai from '../../img/cuisines/thai.jpg';
import turkish from '../../img/cuisines/turkish.jpg';

const Cuisines = (props) => {
	return (
		<Fragment>
			<div className='cuisines'>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>African</h1>
						<img src={african} alt='african' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>American</h1>
						<img src={american} alt='american' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>British</h1>
						<img src={british} alt='british' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Caribbean</h1>
						<img src={caribbean} alt='caribbean' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Chinese</h1>
						<img src={chinese} alt='chinese' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Eastern European</h1>
						<img src={easteuropean} alt='easteuropean' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>French</h1>
						<img src={french} alt='french' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Greek</h1>
						<img src={greek} alt='greek' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Indian</h1>
						<img src={indian} alt='indian' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Italian</h1>
						<img src={italian} alt='italian' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Japanese</h1>
						<img src={japanese} alt='japanese' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Korean</h1>
						<img src={korean} alt='korean' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Mexican</h1>
						<img src={mexican} alt='mexican' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Middle Eastern</h1>
						<img src={middleeastern} alt='middleeastern' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Pakistani</h1>
						<img src={pakistani} alt='pakistani' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Portuguese</h1>
						<img src={portuguese} alt='portuguese' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>South American</h1>
						<img src={southamerican} alt='southamerican' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Spanish</h1>
						<img src={spanish} alt='spanish' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Thai and South-East Asia</h1>
						<img src={thai} alt='thai' className='cuisine__image' />
					</Link>
				</div>
				<div className='cuisine'>
					<Link to={`!#`}>
						<h1>Turkish</h1>
						<img src={turkish} alt='turkish' className='cuisine__image' />
					</Link>
				</div>
			</div>
		</Fragment>
	);
};

Cuisines.propTypes = {};

export default Cuisines;
