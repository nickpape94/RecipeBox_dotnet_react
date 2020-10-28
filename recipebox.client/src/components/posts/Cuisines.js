import React, { Fragment } from 'react';
import { Link } from 'react-router-dom';
import { LazyLoadImage } from 'react-lazy-load-image-component';
import PropTypes from 'prop-types';
import Spinner from '../layout/Spinner';
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

const Cuisines = (props) => (
	// const [ picsLoading, picsLoaded ] = useState(false);

	// useEffect(() => {
	// 	setTimeout(() => picsLoaded(true), 250);
	// }, []);

	// if (!picsLoading) {
	// 	return <Spinner />;
	// }

	<Fragment>
		<div className='cuisines'>
			<div className='cuisine'>
				<Link to={`/cuisines/african`}>
					<h1>African</h1>
					<LazyLoadImage src={african} alt='african' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/american`}>
					<h1>American</h1>
					<LazyLoadImage src={american} alt='american' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/british`}>
					<h1>British</h1>
					<LazyLoadImage src={british} alt='british' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/caribbean`}>
					<h1>Caribbean</h1>
					<LazyLoadImage src={caribbean} alt='caribbean' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/chinese`}>
					<h1>Chinese</h1>
					<LazyLoadImage src={chinese} alt='chinese' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/east%20european`}>
					<h1>Eastern European</h1>
					<LazyLoadImage src={easteuropean} alt='easteuropean' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/french`}>
					<h1>French</h1>
					<LazyLoadImage src={french} alt='french' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/greek`}>
					<h1>Greek</h1>
					<LazyLoadImage src={greek} alt='greek' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/indian`}>
					<h1>Indian</h1>
					<LazyLoadImage src={indian} alt='indian' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/italian`}>
					<h1>Italian</h1>
					<LazyLoadImage src={italian} alt='italian' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/japanese`}>
					<h1>Japanese</h1>
					<LazyLoadImage src={japanese} alt='japanese' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/korean`}>
					<h1>Korean</h1>
					<LazyLoadImage src={korean} alt='korean' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/mexican`}>
					<h1>Mexican</h1>
					<LazyLoadImage src={mexican} alt='mexican' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/middle%20eastern`}>
					<h1>Middle Eastern</h1>
					<LazyLoadImage src={middleeastern} alt='middleeastern' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/pakistani`}>
					<h1>Pakistani</h1>
					<LazyLoadImage src={pakistani} alt='pakistani' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/portuguese`}>
					<h1>Portuguese</h1>
					<LazyLoadImage src={portuguese} alt='portuguese' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/south%20american`}>
					<h1>South American</h1>
					<LazyLoadImage src={southamerican} alt='southamerican' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/spanish`}>
					<h1>Spanish</h1>
					<LazyLoadImage src={spanish} alt='spanish' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/`}>
					<h1>Thai and South-East Asia</h1>
					<LazyLoadImage src={thai} alt='thai' className='cuisine__image' />
				</Link>
			</div>
			<div className='cuisine'>
				<Link to={`/cuisines/turkish`}>
					<h1>Turkish</h1>
					<LazyLoadImage src={turkish} alt='turkish' className='cuisine__image' />
				</Link>
			</div>
		</div>
	</Fragment>
);

export default Cuisines;
