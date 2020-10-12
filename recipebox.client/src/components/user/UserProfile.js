import React, { useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';

const UserProfile = ({ getUser, user: { user, loading }, favourites: { favourites, favouritesLoading }, match }) => {
	useEffect(
		() => {
			getUser(match.params.id);
		},
		[ getUser, match.params.id ]
	);

	return (
		<Fragment>
			{user === null || loading || favouritesLoading ? (
				<Spinner />
			) : (
				<Fragment>
					<h1>{user.username}</h1>
					<h1>{user.lastActive}</h1>
				</Fragment>
			)}
		</Fragment>
	);
};

UserProfile.propTypes = {
	getUser: PropTypes.func.isRequired,
	user: PropTypes.object.isRequired,
	favourites: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	favourites: state.favourites,
	user: state.user
});

export default connect(mapStateToProps, { getUser })(UserProfile);
