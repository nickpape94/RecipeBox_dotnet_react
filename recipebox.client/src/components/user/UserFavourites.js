import React, { useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import { getFavourites } from '../../actions/favourite';
import { connect } from 'react-redux';
import Spinner from '../layout/Spinner';

const UserFavourites = ({ getFavourites, favourites: { favourites }, match }) => {
	useEffect(
		() => {
			getFavourites(match.params.id);
		},
		[ getFavourites, match.params.id ]
	);

	favourites.forEach(() => console.log('favor'));

	return <h1>Persons favourites</h1>;
};

UserFavourites.propTypes = {
	getFavourites: PropTypes.func.isRequired,
	favourites: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	favourites: state.favourites
});

export default connect(mapStateToProps, { getFavourites })(UserFavourites);
