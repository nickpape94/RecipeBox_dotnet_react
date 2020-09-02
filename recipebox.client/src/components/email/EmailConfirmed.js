import React, { useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { propTypes } from 'react-bootstrap/esm/Image';
import { getUser } from '../../actions/user';
import auth from '../../reducers/auth';

const EmailConfirmed = ({ auth }) => {
	// console.log(auth);
	return (
		<Fragment>
			{auth.isAuthenticated &&
			auth.loading === false && (
				<div>
					<h1>Welcome {auth.user.username}! Thanks for confirming your email.</h1>
				</div>
			)}
		</Fragment>
	);
};

EmailConfirmed.propTypes = {
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, {})(EmailConfirmed);
