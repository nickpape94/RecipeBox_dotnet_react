import React from 'react';
import PropTypes from 'prop-types';

const Cuisine = ({ match }) => {
	console.log(match.params.cuisine);

	return (
		<div>
			<h1>{match.params.cuisine}</h1>
		</div>
	);
};

Cuisine.propTypes = {};

export default Cuisine;
